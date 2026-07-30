import {  useNavigate } from 'react-router'
import { useEffect } from 'react'
import type { ApiError } from '../api/types'
import UploadDropzone from '../components/sgpa/UploadDropzone'
import { useUploadPdf } from '../hooks/useUploadPdf'
import { useSgpaStore } from '../store/useSgpaStore'

// displaying the error
function getErrorMessage(error: ApiError | null): string | null {
  if (!error) return null
  switch (error.statusCode) {
    case 400: return error.message
    case 413: return 'File too large. VTU result PDFs are usually under 500 KB. Check you uploaded the correct file.'
    case 422: return 'This does not look like a VTU result PDF. Download your result from results.vtu.ac.in and try again.'
    case 429: return `Too many uploads. Wait ${error.retryAfter ?? '60 seconds'} before trying again.`
    case 503: return 'The processing service is currently unavailable. Please try again in a moment.'
    case 504: return 'Processing timed out. Try again — large PDFs occasionally take longer on the first request.'
    default: return `Something went wrong (${error.statusCode}).${error.requestId ? ` Reference: ${error.requestId}` : ''}`
  }
}

export default function HomePage() {
  const navigate = useNavigate()
  const sgpaResult = useSgpaStore((s) => s.sgpaResult)

  // ✅ SMART NAV FIX: If SGPA data already exists in global store, instantly send them back to results
  useEffect(() => {
    if (sgpaResult) {
      navigate('/results', { replace: true })
    }
  }, [ sgpaResult, navigate ])

  // Silent wake-up logic with explicit browser developer logs
  useEffect(() => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL
    if (baseUrl) {
      fetch(`${baseUrl}/api/sgpa/ping`)
        .then((res) => res.json())
        .then((data) => {
          console.log('%c[Dev Log] Pre-warming sequence completed successfully! Server is live.', 'color: #15803d; font-weight: bold;', data);
        })
        .catch((err) => {
          console.warn('%c[Dev Warning] Pre-warming ping failed or server is cold-starting. Initializing background wake-up routine.', 'color: #b45309; font-weight: bold;', err);
        });
    }
  }, []);

  const { upload, isPending, isError, error, reset } = useUploadPdf()

  function handleUpload(file: File) {
    if (isError) reset() // clear previous error before retrying
    upload(file)
  }

  const errorMessage = isError ? getErrorMessage(error) : null

  // If data exists, prevent rendering the page flashing artifacts during redirect execution
  if (sgpaResult) return null

  return (
    <div className="w-full flex flex-col items-center px-4 sm:px-6 py-12 sm:py-16">

      {/* Hero Section */}
      <div className="flex flex-col items-center text-center mb-10 max-w-content w-full">
        <div className="badge mb-5">
          <span className="w-1.5 h-1.5 rounded-sm bg-brand-800 inline-block" />
          VTU 22 Scheme · All Branches
        </div>
        <h1 className="text-h1 sm:text-display font-bold text-ink-primary tracking-tight mb-4">
          Calculate Your SGPA
        </h1>
        <p className="text-body text-ink-secondary max-w-md">
          Upload your official VTU result PDF and get your SGPA, grade breakdown, and credit summary in seconds.
        </p>
      </div>

      {/* Upload Zone */}
      <UploadDropzone
        onUpload={handleUpload}
        isPending={isPending}
        error={errorMessage}
      />

      {/* Error Reference ID */}
      {isError && error?.requestId && (
        <p className="mt-3 text-caption text-ink-subtle text-center">
          Reference:{' '}
          <span className="font-mono bg-cream-200 px-1.5 py-0.5 rounded text-[11px]">
            {error.requestId}
          </span>
        </p>
      )}

      {/* Action Links (Demo PDF ) */}
      <div className="mt-6 flex flex-wrap items-center justify-center gap-x-6 gap-y-2 text-body-sm text-ink-muted py-2 text-center">
        <div>
          No result file?{' '}
          <a
            href="/demo-vtu-result.pdf"
            download="demo-vtu-result.pdf"
            className="text-brand-800 font-medium underline underline-offset-2 bg-transparent border-0 p-0 cursor-pointer hover:opacity-80 inline"
          >
            Download Demo PDF to test
          </a>
        </div>
      </div>

      {/* How it works with hover animations */}
      <div className="mt-14 sm:mt-16 max-w-card w-full">
        <h2 className="text-h4 font-semibold text-ink-primary mb-5">How it works</h2>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4">
          {[
            { n: '01', title: 'Get your result PDF', desc: 'Log in at results.vtu.ac.in with your USN and download your result.' },
            { n: '02', title: 'Drop it here', desc: 'Drop the PDF above or click to browse from your device.' },
            { n: '03', title: 'See your SGPA', desc: 'Instant SGPA, grade points, and a full subject breakdown.' },
          ].map(item => (
            <div
              key={item.n}
              className="card p-5 border border-black/5 hover:-translate-y-1 hover:border-brand-800/30 hover:shadow-md transition-all duration-200 bg-white"
            >
              <div className="text-caption font-mono text-ink-muted mb-2">{item.n}</div>
              <p className="text-body-sm font-semibold text-ink-primary mb-1">{item.title}</p>
              <p className="text-caption text-ink-muted">{item.desc}</p>
            </div>
          ))}
        </div>
      </div>

    </div>
  )
}
