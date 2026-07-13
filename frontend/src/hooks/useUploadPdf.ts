

import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router'
import { uploadPdf } from '../api/sgpa'
import { useSgpaStore } from '../store/useSgpaStore'
import type { ApiError } from '../api/types'

export function useUploadPdf() {
  const navigate = useNavigate()
  const setSgpaResult = useSgpaStore((s) => s.setSgpaResult)

  const mutation = useMutation({
    mutationFn: async (file: File) => {
      const result = await uploadPdf(file)
      return { result, fileName: file.name }
    },
    onSuccess: ({ result, fileName }) => {
      setSgpaResult(result, fileName)
      navigate('/results')
    },
  })

  return {
    upload: mutation.mutate,
    isPending: mutation.isPending,
    isError: mutation.isError,
    error: mutation.error as ApiError | null,
    reset: mutation.reset,
  }
}
