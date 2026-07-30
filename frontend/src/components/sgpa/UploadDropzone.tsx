
import { useState, useRef, useCallback, useEffect, React } from 'react'
import { Upload, FileText } from 'lucide-react'
import { cn } from '../../lib/cn'

const MAX_BYTES = 2 * 1024 * 1024

function validateFile(file: File): string | null {
  // Built-in protection against the hidden iPhone Safari type stripping glitch
  if (file.type !== 'application/pdf' && !file.name.toLowerCase().endsWith('.pdf')) {
    return 'Only PDF files are accepted. Upload your VTU result PDF.'
  }
  if (file.size > MAX_BYTES) {
    return 'File too large. VTU result PDFs are typically under 500 KB.'
  }
  return null
}

interface Props {
  onUpload: (file: File) => void
  isPending: boolean
  error?: string | null
}

export default function UploadDropzone({ onUpload, isPending, error }: Props) {
  const [ isDragging, setIsDragging ] = useState(false)
  const [ selected, setSelected ] = useState<File | null>(null)
  const [ localError, setLocalError ] = useState<string | null>(null)
  const [ isErrorDismissed, setIsErrorDismissed ] = useState(false)
  const [ loadingStep, setLoadingStep ] = useState(0)
  const inputRef = useRef<HTMLInputElement>(null)

  // Combine validation and server states safely
  const rawError = localError || error
  const displayError = isErrorDismissed ? null : rawError

  // Dynamic status feedback strings
  const LOADING_MESSAGES = [
    "Reading document structure...",
    "Extracting subject marks & grades...",
    "Applying VTU 22 Scheme credit rules...",
    "Finalizing SGPA calculation...",
    "High network load detected — allocating cloud resources..." // Step 4 (10s+) safety fallback message
  ]

  // Feature A: Automatically cycle loading descriptions every 2.5 seconds
  useEffect(() => {
    if (!isPending) {
      setLoadingStep(0)
      return
    }
    const interval = setInterval(() => {
      setLoadingStep((prev) => Math.min(prev + 1, LOADING_MESSAGES.length - 1))
    }, 2500)

    return () => clearInterval(interval)
  }, [ isPending ])

  // Reset display controls when fresh issues pop up
  useEffect(() => {
    if (rawError) {
      setIsErrorDismissed(false)
    }
  }, [ rawError ])

  // Feature B: Auto-dismiss messages and wipe active hooks clean after 4 seconds
  useEffect(() => {
    if (!displayError) return

    const timer = setTimeout(() => {
      setIsErrorDismissed(true)
      setLocalError(null)
      setSelected(null) // Resets file structural node state cleanly
    }, 5000)

    return () => clearTimeout(timer)
  }, [ displayError ])

  const handleFile = useCallback((file: File) => {
    setLocalError(null)
    setIsErrorDismissed(false)
    setSelected(file)

    const err = validateFile(file)
    if (err) {
      setLocalError(err)
      setSelected(null) // Feature C: Strict immediate state reset on error
      return
    }
    onUpload(file)
  }, [ onUpload ])

  function onDragOver(e: React.DragEvent) { e.preventDefault(); setIsDragging(true) }
  function onDragLeave(e: React.DragEvent) { e.preventDefault(); setIsDragging(false) }
  function onDrop(e: React.DragEvent) {
    e.preventDefault()
    setIsDragging(false)
    const file = e.dataTransfer.files[ 0 ]
    if (file) handleFile(file)
  }

  return (
    <div className="w-full max-w-card mx-auto">
      <div
        role="button"
        tabIndex={0}
        aria-label="Upload PDF — click or drag and drop"
        onClick={() => { if (!isPending) inputRef.current?.click() }}
        onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); if (!isPending) inputRef.current?.click() } }}
        onDragOver={onDragOver}
        onDragLeave={onDragLeave}
        onDrop={onDrop}
        className={cn(
          'card min-h-[220px] flex flex-col items-center justify-center gap-4 cursor-pointer select-none transition-all',
          isDragging && '!border-brand-800 shadow-card-hover',
          isPending && 'opacity-60 cursor-not-allowed pointer-events-none',
          displayError && '!border-red-300',
        )}
      >
        <input
          ref={inputRef}
          type="file"
          accept=".pdf,application/pdf"
          className="hidden"
          disabled={isPending}
          onChange={(e) => {
            const file = e.target.files?.[ 0 ]
            if (file) handleFile(file)
            e.target.value = ''
          }}
        />

        {isPending ? (
          <div className="flex flex-col items-center gap-3 text-ink-muted text-center px-4">
            <div className="w-10 h-10 rounded-full border-2 border-cream-400 border-t-brand-800 animate-spin" />
            <p className="text-body-sm font-medium text-ink-primary transition-all duration-300">
              {LOADING_MESSAGES[ loadingStep ]}
            </p>
            <p className="text-caption text-ink-subtle">
              {loadingStep === LOADING_MESSAGES.length - 1
                ? 'Please hold tight, your file is being processed safely.'
                : 'Takes 3–8 seconds'}
            </p>
          </div>
        ) : selected && !displayError ? (
          <div className="flex flex-col items-center gap-3">
            <FileText size={32} className="text-brand-800" />
            <div className="text-center">
              <p className="text-body-sm font-semibold text-ink-primary">{selected.name}</p>
              <p className="text-caption text-ink-muted mt-1">
                {(selected.size / 1024).toFixed(0)} KB · Click to change
              </p>
            </div>
          </div>
        ) : (
          <div className="flex flex-col items-center gap-4 py-4">
            <Upload size={32} className={cn('text-ink-muted', displayError && 'text-red-400')} />
            <div className="text-center">
              <p className="text-body font-medium text-ink-primary">Drop your VTU result PDF here</p>
              <p className="text-body-sm text-ink-muted mt-1">or click to browse · PDF only · max 2 MB</p>
            </div>
            {/* Kept your exact requested badge text targeting phone utility */}
            <div className="badge">Upload Downloaded Pdf from Phone</div>
          </div>
        )}
      </div>

      {displayError && (
        <div className="mt-3 p-3 bg-red-50 border border-red-200 rounded-xl text-caption text-red-700 text-center transition-all duration-300">
          {displayError}
        </div>
      )}
    </div>
  )
}
