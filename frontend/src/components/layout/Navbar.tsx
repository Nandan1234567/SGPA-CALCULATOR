

import { useState } from 'react'
import { Calculator, Menu, X } from 'lucide-react'
import { FaGithub } from 'react-icons/fa'
import { NavLink, Link } from 'react-router'
import { cn } from '../../lib/cn'
import { useSgpaStore } from '../../store/useSgpaStore'

const NAV_LINKS = [
  { to: '/', label: 'Calculator', end: true },
  { to: '/cgpa', label: 'CGPA', end: false },
  { to: '/guide', label: 'Guide', end: false },
]

export default function Navbar() {
  const [ open, setOpen ] = useState(false)

//for navigation to home
  const clearResult = useSgpaStore((s) => s.clearResult)

  return (
    <>
      <header className="sticky top-0 z-50 bg-cream-300 border-b border-[rgba(0,0,0,0.08)]">
        {/* 1. Removed 'max-w-page' and added 'relative' so the nav fills 100% of the laptop monitor with no empty gaps on the right */}
        <nav
          className="w-full mx-auto px-4 sm:px-6 md:px-8 h-16 flex items-center justify-between relative"
          aria-label="Main navigation"
        >
          {/* 2. Logo kept 100% SEPARATE and untouched as a direct child of nav */}
          <Link
            to="/"
            onClick={() => clearResult()}
            className="flex items-center gap-2.5 no-underline shrink-0"
            aria-label="SGPA Calculator home"
          >
            <div className="w-9 h-9 rounded-md flex items-center justify-center bg-brand-800 shrink-0">
              <Calculator size={16} className="text-ink-inverse" />
            </div>
            <span className="text-h4 font-bold text-ink-primary tracking-tight leading-none">
              SGPA Calculator <span className="hidden xs:inline">Calculator</span>
            </span>
          </Link>

          {/* 3. Desktop links mathematically locked to the exact dead-center of the screen */}
          <div className="hidden md:flex items-center gap-2 absolute left-1/2 -translate-x-1/2">
            {NAV_LINKS.map(({ to, label, end }) => (
              <NavLink
                key={to}
                to={to}
                end={end}
                className={({ isActive }) =>
                  cn(
                    // Base classes & Hover effects apply to ALL links at all times
                    'px-3 py-1.5 rounded-md text-sm font-medium transition-colors hover:bg-cream-200 hover:text-ink-primary',
                    isActive
                      ? 'text-ink-primary' // Active state: Just darker text, no background box
                      : 'text-ink-secondary' // Inactive state: Muted text
                  )
                }
              >
                {label}
              </NavLink>
            ))}
          </div>

          <div className="flex items-center gap-1.5 sm:gap-2">
            <a
              href="https://github.com/Nandan1234567/SGPA-CALCULATOR"
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center justify-center w-9 h-9 rounded-md text-ink-muted hover:text-ink-primary hover:bg-cream-200 transition-colors"
              aria-label="View source on GitHub"
            >
              <FaGithub size={19} />
            </a>

            <button
              className="md:hidden flex items-center justify-center w-9 h-9 rounded-md text-ink-secondary hover:bg-cream-200 transition-colors"
              aria-label={open ? 'Close menu' : 'Open menu'}
              aria-expanded={open}
              onClick={() => setOpen((v) => !v)}
            >
              {open ? <X size={20} /> : <Menu size={20} />}
            </button>
          </div>
        </nav>
      </header>

      {/* Mobile drawer backdrop overlay */}
      {open && (
        <div
          className="md:hidden fixed inset-0 z-40"
          onClick={() => setOpen(false)}
          aria-hidden="true"
        >
          <div className="absolute inset-0 bg-black/20" />
        </div>
      )}

      {/* Mobile slide-down menu */}
      <div
        className={cn(
          'md:hidden fixed top-16 left-0 right-0 z-40 bg-cream-300 border-b border-[rgba(0,0,0,0.08)] shadow-dropdown transition-all duration-200 origin-top',
          open ? 'opacity-100 scale-y-100 pointer-events-auto' : 'opacity-0 scale-y-95 pointer-events-none'
        )}
      >
        <div className="px-4 py-4 flex flex-col gap-1">
          {NAV_LINKS.map(({ to, label, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              onClick={() => setOpen(false)}
              className={({ isActive }) =>
                cn(
                  'px-4 py-3 rounded-lg text-body-sm transition-colors',
                  isActive
                    ? 'bg-brand-800 text-ink-inverse font-semibold'
                    : 'text-ink-secondary hover:bg-cream-200'
                )
              }
            >
              {label}
            </NavLink>
          ))}
        </div>
      </div>
    </>
  )
}

