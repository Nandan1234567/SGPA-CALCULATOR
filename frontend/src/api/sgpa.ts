// The actual API call functions for the SGPA feature.


import { postFormData, postJson } from './client'
import type { SgpaResponse, SgpaRequest } from './types'


export async function uploadPdf(file: File): Promise<SgpaResponse> {
  const formData = new FormData()
// FormData: the browser standard for encoding file uploads.

  formData.append('pdf', file, file.name)
  
  //   In SgpaController.cs: "public async Task<ActionResult<SgpaResponse>> FromPdf(IFormFile? pdf)"


  return postFormData<SgpaResponse>('/api/sgpa/from-pdf', formData)
}


export async function calculateSgpa(request: SgpaRequest): Promise<SgpaResponse> {
  return postJson<SgpaRequest, SgpaResponse>('/api/sgpa/calculate', request)
}
