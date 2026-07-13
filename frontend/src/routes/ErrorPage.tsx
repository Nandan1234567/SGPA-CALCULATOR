import { useRouteError, Link } from 'react-router'

export default function ErrorPage() {
  // Catch the routing or rendering error details from React Router
  const error = useRouteError() as any

  return (
    <div className="w-full min-h-[70vh] flex flex-col items-center justify-center px-4 sm:px-6 py-12 text-center">

      {/* 1. Simple, understandable error badge */}
      <div className="badge mb-5 bg-red-100 text-red-800 border border-red-200 font-medium">
        <span className="w-1.5 h-1.5 rounded-full bg-red-600 inline-block mr-1.5" />
        Unexpected Display Glitch
      </div>

      {/* 2. Clear, non-intimidating heading */}
      <h1 className="text-h2 sm:text-h1 font-bold text-ink-primary tracking-tight mb-3">
        Oops! Something Went Wrong
      </h1>

      {/* 3. Actionable, helpful guidance for normal users */}
      <p className="text-body text-ink-secondary mb-6 max-w-md">
        We ran into a temporary browser rendering issue. Don't worry, your result PDF is completely safe. Please try reloading the page, or return home to start fresh.
      </p>

      {/* 4. Pure White Error Box for crisp contrast against the beige page */}
      {error && (
        <div className="w-full max-w-sm bg-white border border-[rgba(0,0,0,0.12)] p-3.5 rounded-lg font-mono text-caption text-ink-muted mb-8 overflow-x-auto text-left select-all shadow-sm">
          <span className="font-bold block text-ink-secondary mb-1">Error Log:</span>
          {error.statusText || error.message || 'Unknown runtime exception'}
        </div>
      )}

      {/* 5. Perfectly Balanced Buttons (Both look crisp, pill-shaped, and interactive!) */}
      <div className="flex flex-wrap items-center justify-center gap-3.5">
        <button
          onClick={() => window.location.reload()}
          className="bg-white border border-brand-800 text-brand-800 hover:bg-brand-800 hover:text-white font-medium px-4 py-2.5 rounded-full transition-all duration-200 no-underline shadow-sm flex items-center justify-center"
        >
          Reload Calculator
        </button>

        <Link
          to="/"
          className="bg-white border border-brand-800 text-brand-800 hover:bg-brand-800 hover:text-white font-medium px-6 py-2.5 rounded-full transition-all duration-200 no-underline shadow-sm flex items-center justify-center"
        >
          Back to Home
        </Link>
      </div>

    </div>
  )
}
