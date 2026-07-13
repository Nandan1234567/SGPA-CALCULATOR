import { type ReactNode } from 'react'
import { Outlet } from 'react-router'
import Footer from './Footer'
import Navbar from './Navbar'

export default function Layout({ children }: { children?: ReactNode }) {
  return (



    <div className="min-h-screen min-h-dvh w-full flex flex-col  bg-cream-300 font-sans">

      <Navbar />

      <main className="grow shrink-0 w-full">

        {children || <Outlet />}
      </main>
      <Footer />
    </div>





  )
}
