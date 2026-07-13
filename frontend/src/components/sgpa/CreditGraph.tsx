import {
  Bar,
  BarChart,
  Cell,
  Label,
  ReferenceLine, ResponsiveContainer,
  Tooltip,
  XAxis, YAxis,
} from 'recharts'
import type { SubjectResultDto } from '../../api/types'

interface ChartPoint {
  code: string
  fullCode: string
  name: string
  gp: number
  isPass: boolean
  isNonCredit: boolean
  isIncluded: boolean
  grade: string
}

function barColor(p: ChartPoint): string {
  // if (p.isNonCredit) return '#c5d4cf' // grey
  if (p.isNonCredit) return 'rgba(26, 58, 42, 0.35)' // grey

  if (!p.isPass) return '#b91c1c' //red

  return 'rgba(26, 58, 42, 1)' // rest green
}

function TooltipBox({ active, payload }: { active?: boolean; payload?: Array<{ payload: ChartPoint }> }) {
  if (!active || !payload?.length) return null
  const d = payload[ 0 ].payload
  return (
    <div style={{
      background: '#ffffff', border: '1px solid rgba(0,0,0,0.12)',
      borderRadius: '10px', padding: '10px 14px',
      boxShadow: '0 8px 24px rgba(0,0,0,0.12)', maxWidth: '180px',
    }}>
      <p style={{ fontFamily: 'Space Grotesk', fontSize: '11px', fontWeight: 700, color: '#1a3a2a', marginBottom: '2px' }}>
        {d.fullCode}
      </p>
      {d.name && (
        <p style={{ fontFamily: 'Space Grotesk', fontSize: '10px', color: '#7a9080', marginBottom: '6px' }}>
          {d.name.length > 32 ? d.name.slice(0, 30) + '…' : d.name}
        </p>
      )}
      <p style={{ fontFamily: 'Space Grotesk', fontSize: '20px', fontWeight: 700, color: '#1a3a2a' }}>
        {d.gp.toFixed(1)}
        <span style={{ fontSize: '11px', fontWeight: 400, color: '#7a9080' }}> / 10</span>
      </p>
      <p style={{ fontFamily: 'Space Grotesk', fontSize: '10px', color: '#7a9080', marginTop: '2px' }}>
        Grade: <strong style={{ color: '#1a3a2a' }}>{d.grade}</strong>
      </p>
    </div>
  )
}

interface Props { subjects: SubjectResultDto[] }

export default function CreditGraph({ subjects }: Props) {
  const data: ChartPoint[] = subjects
    .filter((s) => !s.isUnresolved)
    .map((s) => ({
      code: s.subjectCode.length > 8 ? s.subjectCode.slice(0, 7) + '…' : s.subjectCode,
      fullCode: s.subjectCode,
      name: s.subjectName,
      gp: s.gradePoints,
      isPass: s.isPass,
      isIncluded: s.isIncludedInSgpa,
      isNonCredit: s.isNonCreditForSgpa,
      grade: s.grade,
    }))

  if (!data.length) return null

  return (
    <div className="w-full overflow-x-auto pb-4">
      <div className="min-w-[500px] h-[280px] mt-6">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={data} barSize={26} margin={{ top: 8, right: 4, bottom: 0, left: -20 }}>

            <XAxis
              dataKey="code"
              tick={{ fontSize: 9, fill: '#7a9080', fontFamily: 'Space Grotesk' }}
              axisLine={false}
              tickLine={false}
            />

            <YAxis
              domain={[ 0, 10 ]}
              ticks={[ 0, 4, 6, 8, 10 ]}
              tick={{ fontSize: 9, fill: '#7a9080', fontFamily: 'Space Grotesk' }}
              width={55}
              axisLine={false}
              tickLine={false}
            />
            <Label
              value="Grade Points"
              angle={-90}
              position="insideLeft"
              dx={-25} //  2. This pushes the text 25 pixels to the left!
              style={{
                textAnchor: 'middle',
                fill: '#7a9080',
                fontSize: 12,
                fontWeight: 500
              }}
            />

            <Tooltip content={<TooltipBox />} cursor={{ fill: 'rgba(26,58,42,0.04)' }} />
            <ReferenceLine y={4} stroke="#b91c1c" strokeDasharray="4 2" strokeWidth={1.5} />
            <Bar dataKey="gp" radius={[ 4, 4, 0, 0 ]}>
              {data.map((item, i) => (
                <Cell key={i} fill={barColor(item)} />
              ))}
            </Bar>
          </BarChart>

        </ResponsiveContainer>
      </div>
    </div>
  )
}


