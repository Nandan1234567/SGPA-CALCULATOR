import { VTU_22_BRANCHES } from '../../constants/vtuCredits'

interface Props {
  studentName: string
  onStudentNameChange: (v: string) => void
  selectedBranch: string
  onBranchChange: (code: string) => void
}

export default function StudentInfoForm({
  studentName, onStudentNameChange, selectedBranch, onBranchChange,
}: Props) {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.04)]">
      <div>
        <label className="block text-[10px] font-semibold text-ink-muted uppercase tracking-wide mb-1.5">
          Student Name <span className="font-normal normal-case">(optional)</span>
        </label>
        <input
          type="text"
          value={studentName}
          onChange={(e) => onStudentNameChange(e.target.value)}
          placeholder="Your full name"
          className="input text-body-sm"
          maxLength={80}
        />
      </div>

      <div>
        <label className="block text-[10px] font-semibold text-ink-muted uppercase tracking-wide mb-1.5">
          Branch <span className="text-red-400">*</span>
          <span className="font-normal normal-case ml-1 text-ink-subtle">(auto-fills credits)</span>
        </label>
        <select
          value={selectedBranch}
          onChange={(e) => onBranchChange(e.target.value)}
          className="input text-body-sm"
        >
          <option value="">Select your branch</option>
          {VTU_22_BRANCHES.map((b) => (
            <option key={b.code} value={b.code}>{b.code} — {b.name}</option>
          ))}
        </select>
      </div>
    </div>
  )
}
