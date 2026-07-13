

import { create } from 'zustand'
import type { SgpaResponse } from '../api/types'

interface SgpaStore {
  sgpaResult: SgpaResponse | null
  uploadedFileName: string | null
  setSgpaResult: (result: SgpaResponse, fileName?: string) => void
  clearResult: () => void
}

export const useSgpaStore = create<SgpaStore>((set) => ({
  sgpaResult: null,
  uploadedFileName: null,

  setSgpaResult: (result, fileName) => set({
    sgpaResult: result,
    uploadedFileName: fileName ?? null,
  }),

  clearResult: () => set({
    sgpaResult: null,
    uploadedFileName: null,
  }),
}))
