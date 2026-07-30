import { ApiError, type ApiErrorResponse, type RateLimitErrorResponse } from './types'

const BASE_URL = import.meta.env.VITE_API_BASE_URL

if (!BASE_URL) {
  throw new Error('VITE_API_BASE_URL is not set. Create .env.local with:\nVITE_API_BASE_URL=http://localhost:5100')
}

async function request<T>(path: string, options: RequestInit, timeoutMs = 20000): Promise<T> {
  // 1. Instant Offline Check: Fail immediately before making a network call if disconnected
  if (!navigator.onLine) {
    throw new ApiError(
      0,
      'You appear to be offline. Please check your internet connection and try again.'
    )
  }

  // 2. Setup AbortController for the 20-second timeout
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), timeoutMs)

  let response: Response
  try {
    response = await fetch(`${BASE_URL}${path}`, {
      ...options,
      signal: controller.signal,
    })
  } catch (err: unknown) {
    // 3. Handle 20-second timeout cancellation
    if (err instanceof DOMException && err.name === 'AbortError') {
      throw new ApiError(
        408,
        'The request took longer than 20 seconds and was cancelled. The server might be busy—please try again after some time!'
      )
    }

    // 4. Handle connection drops that occurred while the request was in flight
    if (!navigator.onLine) {
      throw new ApiError(
        0,
        'You appear to be offline. Please check your internet connection and try again.'
      )
    }

    // 5. Handle unreachable server / waking up cloud instances
    throw new ApiError(
      503,
      'Cannot reach the server. If this is the first request, the cloud server may be waking up. Please try again in 30 seconds!'
    )
  } finally {
    // 6. Cleanup: Clear timer immediately once fetch completes or fails so it doesn't leak memory
    clearTimeout(timeoutId)
  }

  // --- Response Parsing & API Error Handling ---

  if (response.ok) {
    return (await response.json()) as T
  }

  let errorBody: unknown
  try {
    errorBody = await response.json()
  } catch {
    throw new ApiError(response.status, `Server error ${response.status}`)
  }

  const status = response.status

  if (status === 429) {
    const body = errorBody as RateLimitErrorResponse
    throw new ApiError(429, body.error ?? 'Too many requests', undefined, body.retryAfter)
  }

  const body = errorBody as ApiErrorResponse
  throw new ApiError(status, body.error ?? `Request failed (${status})`, body.requestId)
}

export async function postFormData<T>(path: string, formData: FormData): Promise<T> {
  return request<T>(path, { method: 'POST', body: formData })
}

export async function postJson<TBody, TResponse>(path: string, body: TBody): Promise<TResponse> {
  return request<TResponse>(path, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
}
