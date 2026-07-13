import {
  Bar, BarChart, Cell, Label,
  ReferenceLine, ResponsiveContainer,
  Tooltip, XAxis, YAxis,
} from 'recharts'

export interface SemPoint {
  label: string
  sgpa: number
  credits: number
}

function barColor(sgpa: number): string {
  if (sgpa < 5.0) return '#b91c1c'
  if (sgpa < 6.0) return 'rgba(26, 58, 42, 0.35)'
  return 'rgba(26, 58, 42, 1)'
}

function TooltipBox({
  active,
  payload,
}: {
  active?: boolean
  payload?: Array<{ payload: SemPoint }>
}) {
  if (!active || !payload?.length) return null
  const d = payload[ 0 ].payload
  return (
    <div style={{
      background: '#ffffff',
      border: '1px solid rgba(0,0,0,0.12)',
      borderRadius: '10px',
      padding: '10px 14px',
      boxShadow: '0 8px 24px rgba(0,0,0,0.12)',
      minWidth: '130px',
    }}>
      <p style={{ fontFamily: 'Space Grotesk', fontSize: '11px', fontWeight: 700, color: '#1a3a2a', marginBottom: '4px' }}>
        {d.label}
      </p>
      <p style={{ fontFamily: 'Space Grotesk', fontSize: '20px', fontWeight: 700, color: '#1a3a2a', margin: 0 }}>
        {d.sgpa.toFixed(2)}
        <span style={{ fontSize: '11px', fontWeight: 400, color: '#7a9080' }}> SGPA</span>
      </p>
      <p style={{ fontFamily: 'Space Grotesk', fontSize: '10px', color: '#7a9080', marginTop: '2px', marginBottom: 0 }}>
        {d.credits} credits
      </p>
    </div>
  )
}

interface Props {
  data: SemPoint[]
}

export default function SemesterGraph({ data }: Props) {
  if (!data.length) return null

  return (
    <div className="w-full overflow-x-auto pb-4">
      <div className="min-w-[400px] h-[280px] mt-6">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={data} barSize={32} margin={{ top: 8, right: 4, bottom: 0, left: -20 }}>
            <XAxis
              dataKey="label"
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
              value="SGPA"
              angle={-90}
              position="insideLeft"
              dx={-25}
              style={{ textAnchor: 'middle', fill: '#7a9080', fontSize: 12, fontWeight: 500 }}
            />
            <Tooltip content={<TooltipBox />} cursor={{ fill: 'rgba(26,58,42,0.04)' }} />
            <ReferenceLine y={7} stroke="rgba(26,58,42,0.35)" strokeDasharray="4 2" strokeWidth={1.5} />
            <Bar dataKey="sgpa" radius={[ 4, 4, 0, 0 ]}>
              {data.map((item, i) => (
                <Cell key={i} fill={barColor(item.sgpa)} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}
