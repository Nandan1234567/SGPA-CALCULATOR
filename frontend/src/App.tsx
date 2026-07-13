import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { createBrowserRouter, RouterProvider } from 'react-router'

import Layout from './components/layout/Layout'
import CgpaPage from './routes/CgpaPage'
import ErrorPage from './routes/ErrorPage'
import GuidePage from './routes/GuidePage'
import HomePage from './routes/HomePage'
import NotFoundPage from './routes/NotFoundPage'
import ResultsPage from './routes/ResultsPage'

// 1. QueryClient remains outside the component to protect cache memory across renders
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
    },
    mutations: {
      retry: 0,
    },
  },
})

// 2. Data Router Setup with Nested Error Shielding
const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        /*
        
          By placing errorElement here inside <Layout />, if HomePage, ResultsPage,
          or CgpaPage suffers a fatal JavaScript crash, React Router catches it!
          Navbar and Footer STAY intact on screen, and only the middle section
           transforms into <ErrorPage />!
        */
        errorElement: <ErrorPage />,
        children: [
          { index: true, element: <HomePage /> },
          { path: 'results', element: <ResultsPage /> },
          { path: 'cgpa', element: <CgpaPage /> },
          { path: 'guide', element: <GuidePage /> },
          { path: '*', element: <NotFoundPage /> }, // The 404 Catch-All remains at the bottom
        ],
      },
    ],
  },
])

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}
