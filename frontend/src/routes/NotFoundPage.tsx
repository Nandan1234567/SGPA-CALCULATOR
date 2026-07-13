import { Link } from 'react-router'

export default function NotFoundPage() {
  return (
    /*
      1. w-full & min-h-[70vh]: Stretches 100% across the monitor width (eliminating the right gap!) and 70% of the screen height.
      2. flex flex-col items-center justify-center: Locks the badge, title, and button dead-center both horizontally and vertically.
    */
    <div className="w-full min-h-[70vh] flex flex-col items-center justify-center px-4 sm:px-6 py-12 text-center">

      {/* Playful VTU Engineering Badge (Grammar corrected) */}
      <div className="badge mb-5 bg-cream-200 text-ink-secondary border border-[rgba(0,0,0,0.08)]">
        <span className="w-1.5 h-1.5 rounded-sm bg-brand-800 inline-block" />
        Bro, does this page exist?
      </div>

      {/* Giant 404 Display */}
      <div className="text-display sm:text-[4.5rem] font-bold text-ink-disabled mb-2 select-none tracking-tight leading-none">
        404
      </div>

      {/* Your Requested VTU Humor Title (Grammar corrected: Its -> It's) */}
      <h1 className="text-h2 sm:text-h1 font-bold text-ink-primary tracking-tight mb-3">
        Error 404: No, it's out of syllabus!
      </h1>

      {/* Relatable Engineering Description (Grammar & capitalization corrected) */}
      <p className="text-body text-ink-secondary mb-8 max-w-md">
        This URL never existed here! Let's skip this question and get back to calculating your SGPA
      </p>

      {/* Primary Action Button */}
      <Link to="/" className="btn-primary no-underline">
        Back to Calculator
      </Link>

    </div>
  )
}
