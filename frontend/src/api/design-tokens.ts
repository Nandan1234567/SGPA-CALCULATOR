// ─────────────────────────────────────────────────────────────────────────────
//  SGPA+ DESIGN TOKENS
//  Theme : Current + Space Grotesk
//  Use   : import { colors, typography, ... } from '@/design-tokens'
// ─────────────────────────────────────────────────────────────────────────────

// ─── COLORS ──────────────────────────────────────────────────────────────────

export const colors = {
  brand: {
    950: '#0a1c13',
    900: '#122a1e',
    800: '#1a3a2a',   // PRIMARY — use for logo, headings, CTA buttons
    700: '#1a5435',
    600: '#1e6b42',
    500: '#2d8f58',
    400: '#45aa72',
    300: '#75c696',
    200: '#aadcbc',
    100: '#d5eedd',
    50: '#edf7f2',
  },
  cream: {
    50: '#ffffff',
    100: '#fdfcf9',
    200: '#f5f2ea',   // subtle card / alternate bg
    300: '#edeae2',   // PAGE BACKGROUND
    400: '#e0dcd3',
    500: '#ccc8be',
  },
  text: {
    primary: '#1a3a2a',   // headings, nav, strong labels
    secondary: '#2d4a3c',   // body paragraphs
    muted: '#7a9080',   // placeholders, captions, breadcrumbs
    subtle: '#a0b5ab',   // very light labels
    disabled: '#c5d4cf',
    inverse: '#ffffff',   // text on dark brand bg
  },
  border: {
    default: 'rgba(0,0,0,0.09)',
    medium: 'rgba(0,0,0,0.12)',
    strong: 'rgba(26,58,42,0.20)',
    brand: 'rgba(26,58,42,0.30)',
  },
  badge: {
    bg: '#dceee4',
    text: '#1a3a2a',
    dot: '#1a3a2a',
  },
} as const;

// ─── TYPOGRAPHY ───────────────────────────────────────────────────────────────

export const typography = {
  fontFamily: '"Space Grotesk", system-ui, sans-serif',
  googleFontUrl: 'https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@300;400;500;600;700&display=swap',
  weights: {
    light: 300,
    regular: 400,
    medium: 500,
    semibold: 600,
    bold: 700,
  },
  scale: {
    display: { size: '3rem', lh: '1.1', weight: 700, ls: '-0.02em' },
    h1: { size: '2.25rem', lh: '1.15', weight: 700, ls: '-0.01em' },
    h2: { size: '1.75rem', lh: '1.2', weight: 600 },
    h3: { size: '1.375rem', lh: '1.3', weight: 600 },
    h4: { size: '1.125rem', lh: '1.4', weight: 500 },
    body: { size: '1rem', lh: '1.7', weight: 400 },
    bodySm: { size: '0.875rem', lh: '1.6', weight: 400 },
    caption: { size: '0.75rem', lh: '1.4', weight: 400 },
  },
  // Tailwind class shortcuts
  tailwind: {
    display: 'text-display font-bold tracking-tight font-sans',
    h1: 'text-h1 font-bold tracking-tight font-sans',
    h2: 'text-h2 font-semibold font-sans',
    h3: 'text-h3 font-semibold font-sans',
    h4: 'text-h4 font-medium font-sans',
    body: 'text-body font-regular font-sans',
    bodySm: 'text-body-sm font-regular font-sans',
    caption: 'text-caption font-regular font-sans',
  },
} as const;

// ─── SPACING ──────────────────────────────────────────────────────────────────

export const spacing = {
  1: '4px',
  2: '8px',
  3: '12px',
  4: '16px',
  5: '20px',
  6: '24px',
  7: '28px',
  8: '32px',
  10: '40px',
  12: '48px',
  16: '64px',
  20: '80px',
} as const;

// ─── BORDER RADIUS ────────────────────────────────────────────────────────────

export const radius = {
  sm: '8px',     // small interactive elements
  md: '12px',    // inputs, small cards
  lg: '16px',    // main content cards
  xl: '20px',    // large sections
  pill: '9999px',  // buttons, badges, dropdowns
} as const;

// ─── SHADOWS ──────────────────────────────────────────────────────────────────

export const shadows = {
  card: '0 4px 16px rgba(26,58,42,0.08), 0 1px 4px rgba(26,58,42,0.04)',
  cardHover: '0 8px 24px rgba(26,58,42,0.12), 0 2px 8px rgba(26,58,42,0.06)',
  focus: '0 0 0 3px rgba(26,58,42,0.12)',
  dropdown: '0 8px 24px rgba(0,0,0,0.12), 0 2px 8px rgba(0,0,0,0.06)',
  btn: '0 2px 8px rgba(26,58,42,0.20)',
} as const;

// ─── COMPONENT TOKENS ─────────────────────────────────────────────────────────

export const components = {
  button: {
    primary: {
      bg: colors.brand[ 800 ],
      bgHover: colors.brand[ 900 ],
      bgActive: colors.brand[ 950 ],
      text: colors.text.inverse,
      radius: radius.pill,
      padding: '10px 24px',
      fontSize: '0.875rem',
      fontWeight: 600,
      shadow: shadows.btn,
      // Tailwind: "btn-primary" class from globals.css
    },
    ghost: {
      bg: 'transparent',
      bgHover: colors.cream[ 200 ],
      text: colors.text.primary,
      border: `1px solid ${colors.border.medium}`,
      borderHover: `1px solid ${colors.border.strong}`,
      radius: radius.pill,
      padding: '10px 20px',
      fontSize: '0.875rem',
      fontWeight: 500,
      // Tailwind: "btn-ghost" class from globals.css
    },
  },
  card: {
    bg: colors.cream[ 50 ],
    border: `1px solid ${colors.border.default}`,
    radius: radius.lg,
    padding: '24px 28px',
    shadow: shadows.card,
    // Tailwind: "card" class OR "bg-cream-50 border border-[rgba(0,0,0,0.09)] rounded-lg p-6 shadow-card"
  },
  input: {
    bg: colors.cream[ 50 ],
    border: `1px solid ${colors.border.default}`,
    borderFocus: `1px solid ${colors.border.brand}`,
    radius: radius.md,
    padding: '12px 16px',
    fontSize: '0.875rem',
    placeholder: colors.text.muted,
    text: colors.text.primary,
    shadowFocus: shadows.focus,
    // Tailwind: "input" class from globals.css
  },
  badge: {
    bg: colors.badge.bg,
    text: colors.badge.text,
    radius: radius.pill,
    padding: '4px 12px',
    fontSize: '0.75rem',
    fontWeight: 600,
    // Tailwind: "badge" class OR "bg-badge-bg text-badge-text text-caption font-semibold px-3 py-1 rounded-pill"
  },
  dropdown: {
    bg: colors.cream[ 50 ],
    border: `1px solid ${colors.border.medium}`,
    radius: radius.pill,
    padding: '8px 16px',
    fontSize: '0.8125rem',
    text: colors.text.primary,
    shadow: shadows.dropdown,
    // Tailwind: "dropdown-pill" class from globals.css
  },
  nav: {
    bg: colors.cream[ 300 ],
    text: colors.text.primary,
    textOpacity: 0.8,
    activeWeight: 600,
    logoColor: colors.brand[ 800 ],
    logoSize: '1.25rem',
    logoWeight: 700,
    // Tailwind: "nav-link" class from globals.css
  },
  breadcrumb: {
    text: colors.text.muted,
    separator: '>',
    fontSize: '0.75rem',
    // Tailwind: "breadcrumb" class from globals.css
  },
  page: {
    bg: colors.cream[ 300 ],
    maxWidth: '1200px',
    paddingX: '24px',
    paddingY: '0',
  },
} as const;

// ─── FIGMA VARIABLE NAMES (for reference) ─────────────────────────────────────
// When setting up Figma Variables, use these exact names:
//
// Collection: "Brand"
//   brand/800  →  #1a3a2a   (Primary)
//   brand/100  →  #d5eedd   (Brand Light)
//   brand/50   →  #edf7f2   (Brand Lighter)
//
// Collection: "Surface"
//   surface/page    →  #edeae2
//   surface/card    →  #ffffff
//   surface/subtle  →  #f5f2ea
//   surface/badge   →  #dceee4
//
// Collection: "Text"
//   text/primary    →  #1a3a2a
//   text/secondary  →  #2d4a3c
//   text/muted      →  #7a9080
//   text/subtle     →  #a0b5ab
//   text/inverse    →  #ffffff
//
// Collection: "Border"
//   border/default  →  rgba(0,0,0,0.09)
//   border/medium   →  rgba(0,0,0,0.12)
//   border/strong   →  rgba(26,58,42,0.20)
//   border/brand    →  rgba(26,58,42,0.30)
