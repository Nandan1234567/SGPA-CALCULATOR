import { type MouseEvent } from 'react'

const VTU_LINKS = [
  { label: 'VTU Results', href: 'https://results.vtu.ac.in' },
  { label: 'VTU Circulars', href: 'https://vtu.ac.in/circulars' },
  { label: 'Model Papers', href: 'https://vtu.ac.in/exam-corner' },
]

export default function Footer() {
  const handleShare = (e: MouseEvent<HTMLButtonElement>) => {
    e.preventDefault()
    const shareText = 'Check out SGPA Calculator — instant VTU SGPA from your result PDF!'
    if (navigator.share) {
      navigator.share({
        title: 'SGPA Calculator | VTU Grade Tool',
        text: shareText,
        url: window.location.href,
      }).catch(() => { })
    } else {
      const url = `https://api.whatsapp.com/send?text=${encodeURIComponent(`${shareText} ${window.location.href}`)}`
      window.open(url, '_blank', 'noopener,noreferrer')
    }
  }

  return (
    <footer className="border-t border-[rgba(0,0,0,0.08)] bg-cream-200 w-full mt-auto">
      {/* 1. ULTRA-COMPACT SAAS PADDING: Reduced from py-8 to pt-6 pb-4 for a sleek, professional utility-tool feel */}
      <div className="w-full px-4 pt-6 pb-4 sm:px-6 sm:pt-6 sm:pb-4">

        {/* 2. FULL-WIDTH CENTERED COLUMNS: Keeps horizontal centering 100% locked without restrictions */}
        <div className="w-full flex justify-center">
          <div className="flex gap-20 sm:gap-32 text-left">

            {/* Column 1: VTU Resources */}
            <div>
              <h3 className="text-caption font-bold uppercase tracking-wider text-ink-primary mb-1.5">
                VTU Resources
              </h3>
              <ul className="flex flex-col gap-1.5">
                {VTU_LINKS.map((link) => (
                  <li key={link.href}>
                    <a
                      href={link.href}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-caption text-ink-muted hover:text-ink-secondary transition-colors inline-block"
                    >
                      {link.label}
                    </a>
                  </li>
                ))}
              </ul>
            </div>

            {/* Column 2: Support & Share */}
            <div>
              <h3 className="text-caption font-bold uppercase tracking-wider text-ink-primary mb-1.5">
                Support &amp; Share
              </h3>
              <ul className="flex flex-col gap-1.5">
                <li>
                  <a
                    href="https://forms.gle/W9prdxs4vN3daEg1A"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-caption text-ink-muted hover:text-ink-secondary transition-colors inline-flex items-center gap-1"
                  >
                    Report a Bug
                    <span className="text-[10px]">↗</span>
                  </a>
                </li>
                <li>
                  <button
                    onClick={handleShare}
                    className="text-caption text-ink-muted hover:text-ink-secondary transition-colors text-left inline-flex items-center gap-1 cursor-pointer"
                  >
                    Share Tool
                    <span className="text-[10px]">↗</span>
                  </button>
                </li>
                <li>
                  <a
                    href="https://x.com/legend721617"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-caption text-ink-subtle hover:text-ink-primary transition-colors inline-flex items-center gap-1"
                  >
                    Connect with X
                    <span className="text-[10px]">↗</span>
                  </a>
                </li>
              </ul>
            </div>

          </div>
        </div>

        {/* 3. SYMMETRIC DISCLAIMER: Balanced top/bottom spacing around the border line, with flex-centering so the text sits precisely in the middle! */}
        <div className="mt-5 pt-3.5 border-t border-[rgba(0,0,0,0.06)] flex items-center justify-center text-center">
          <p className="text-[11px] text-ink-subtle/80 leading-normal">
            SGPA Calculator is an independent reference tool not affiliated with VTU.
          </p>
        </div>

      </div>

      {/*
        Safe-area spacer — covers iOS home indicator & Android nav gesture bar.
      */}
      <div style={{ height: 'env(safe-area-inset-bottom, 0px) ' }} aria-hidden="true" />
    </footer>
  )
}


// import { type MouseEvent } from 'react'

// const VTU_LINKS = [
//   { label: 'VTU Results', href: 'https://results.vtu.ac.in' },
//   { label: 'VTU Circulars', href: 'https://vtu.ac.in/circulars' },
//   { label: 'Model Papers', href: 'https://vtu.ac.in/exam-corner' },
// ]

// export default function Footer() {
//   const handleShare = (e: MouseEvent<HTMLButtonElement>) => {
//     e.preventDefault()
//     const shareText = 'Check out SGPA Calculator — instant VTU SGPA from your result PDF!'
//     if (navigator.share) {
//       navigator.share({
//         title: 'SGPA Calculator | VTU Grade Tool',
//         text: shareText,
//         url: window.location.href,
//       }).catch(() => { })
//     } else {
//       const url = `https://api.whatsapp.com/send?text=${encodeURIComponent(`${shareText} ${window.location.href}`)}`
//       window.open(url, '_blank', 'noopener,noreferrer')
//     }
//   }

//   return (
//     <footer className="border-t border-black/5 bg-cream-50 w-full mt-auto">
//       {/* Generous padding gives it that enterprise SaaS breathing room */}
//       <div className="w-full max-w-4xl mx-auto px-4 sm:px-6 pt-12 pb-8">

//         {/* Columns Container */}
//         <div className="flex flex-row justify-center gap-16 sm:gap-32 text-left">

//           {/* Column 1: VTU Resources */}
//           <div>
//             <h3 className="text-[11px] font-bold uppercase tracking-wider text-ink-primary mb-4">
//               VTU Resources
//             </h3>
//             <ul className="flex flex-col gap-3">
//               {VTU_LINKS.map((link) => (
//                 <li key={link.href}>
//                   <a
//                     href={link.href}
//                     target="_blank"
//                     rel="noopener noreferrer"
//                     className="text-caption font-medium text-ink-muted hover:text-brand-800 transition-colors inline-block"
//                   >
//                     {link.label}
//                   </a>
//                 </li>
//               ))}
//             </ul>
//           </div>

//           {/* Column 2: Support & Share */}
//           <div>
//             <h3 className="text-[11px] font-bold uppercase tracking-wider text-ink-primary mb-4">
//               Support &amp; Share
//             </h3>
//             <ul className="flex flex-col gap-3">
//               <li>
//                 <a
//                   href="https://forms.gle/W9prdxs4vN3daEg1A"
//                   target="_blank"
//                   rel="noopener noreferrer"
//                   className="text-caption font-medium text-ink-muted hover:text-brand-800 transition-colors inline-flex items-center gap-1"
//                 >
//                   Report a Bug
//                   <span className="text-[10px] text-ink-subtle">↗</span>
//                 </a>
//               </li>
//               <li>
//                 <button
//                   onClick={handleShare}
//                   className="text-caption font-medium text-ink-muted hover:text-brand-800 transition-colors text-left inline-flex items-center gap-1 cursor-pointer"
//                 >
//                   Share Tool
//                   <span className="text-[10px] text-ink-subtle">↗</span>
//                 </button>
//               </li>
//               <li>
//                 <a
//                   href="https://x.com/legend721617"
//                   target="_blank"
//                   rel="noopener noreferrer"
//                   className="text-caption font-medium text-ink-muted hover:text-brand-800 transition-colors inline-flex items-center gap-1"
//                 >
//                   Connect with X
//                   <span className="text-[10px] text-ink-subtle">↗</span>
//                 </a>
//               </li>
//             </ul>
//           </div>

//         </div>

//         {/* Symmetric Disclaimer */}
//         <div className="mt-12 pt-6 border-t border-black/5 flex items-center justify-center text-center">
//           <p className="text-[11px] font-medium text-ink-subtle leading-relaxed">
//             SGPA Calculator is an independent reference tool not affiliated with VTU.
//           </p>
//         </div>

//       </div>

//       {/* Safe-area spacer for mobile */}
//       <div style={{ height: 'env(safe-area-inset-bottom, 0px)' }} aria-hidden="true" />
//     </footer>
//   )
// }
