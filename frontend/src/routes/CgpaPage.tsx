import { Calculator, RotateCcw } from 'lucide-react'
import { Link } from 'react-router'
import { useCgpaForm } from '../components/cgpa/useCgpaForm'
import StudentInfoForm from '../components/cgpa/StudentInfoForm'
import HistoryPanel from '../components/cgpa/HistoryPanel'
import SemesterTable from '../components/cgpa/SemesterTable'
import CgpaResultCard from '../components/cgpa/CgpaResultCard'

export default function CgpaPage() {
  const form = useCgpaForm()

  return (
    <div className="w-full flex flex-col items-center px-4 sm:px-6 py-6 pb-16">
      <div className="w-full max-w-5xl">

        <nav className="breadcrumb mb-5">
          <Link to="/" className="hover:text-ink-secondary transition-colors">Home</Link>
          <span>›</span>
          <span>CGPA Calculator</span>
        </nav>

        <div className="mb-7">
          <div className="badge mb-3">
            <span className="w-1.5 h-1.5 rounded-sm bg-brand-800 inline-block" />
            VTU 22 Scheme · All Branches
          </div>
          <h1 className="text-h1 font-bold text-ink-primary">CGPA Calculator</h1>
          <p className="text-body text-ink-secondary mt-2 max-w-prose">
            Select your branch to auto-fill credits, then enter your SGPA for each completed semester.
          </p>
        </div>

        <div className="card p-0 overflow-hidden mb-5 bg-white border border-black/5 shadow-sm">

          <div className="flex items-center justify-between px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
            <div className="flex items-center gap-2">
              <Calculator size={15} className="text-brand-800" />
              <h2 className="text-body-sm font-semibold text-ink-primary">Semester Results</h2>
            </div>
            {(form.filledCount > 0 || form.selectedBranch) && (
              <button
                onClick={form.handleReset}
                className="flex items-center gap-1.5 text-caption text-ink-muted hover:text-red-500 transition-colors"
              >
                <RotateCcw size={11} />
                Reset all
              </button>
            )}
          </div>

          <StudentInfoForm
            studentName={form.studentName}
            onStudentNameChange={form.setStudentName}
            selectedBranch={form.selectedBranch}
            onBranchChange={form.handleBranchChange}
          />

          <HistoryPanel
            historyByUsn={form.historyByUsn}
            totalHistoryCount={form.totalHistoryCount}
            selectedKeys={form.selectedKeys}
            historyOpen={form.historyOpen}
            setHistoryOpen={form.setHistoryOpen}
            onToggle={form.handleHistoryToggle}
           
          />

          <SemesterTable
            rows={form.rows}
            onRowChange={form.handleRowChange}
            selectedBranch={form.selectedBranch}
          />

          <div className="px-4 sm:px-5 py-4 bg-cream-100/30 border-t border-[rgba(0,0,0,0.04)]">
            <p className="text-[10px] text-ink-subtle mb-4 leading-relaxed">
              Credits auto-filled from your branch (VTU 22 scheme). Values from saved results are exact.
              You can edit any field. Blank rows are skipped.
            </p>
            {form.error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl">
                <p className="text-caption text-red-700">{form.error}</p>
              </div>
            )}
            <div className="flex justify-center">
              <button className="btn-primary px-10" onClick={form.handleCalculate}>
                Calculate CGPA
              </button>
            </div>
          </div>
        </div>

        {form.result && (
          <CgpaResultCard
            result={form.result}
            division={form.division}
            percentage={form.percentage}
          />
        )}

      </div>
    </div>
  )
}
