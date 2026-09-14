import {
  CartesianGrid,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { formatNumber } from '../utils/formatters.js'

function ChartTooltip({ active, payload, label }) {
  if (!active || !payload?.length) {
    return null
  }

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-3 text-xs shadow-lg">
      <p className="mb-2 font-semibold text-slate-900">Semana {label}</p>
      {payload.map((item) => (
        <p key={item.dataKey} className="mt-1" style={{ color: item.color }}>
          {item.name}: <strong>{formatNumber(item.value)}</strong>
        </p>
      ))}
    </div>
  )
}

export function CasesChart({ data }) {
  const chartData = [...data].reverse().map((week) => ({
    semana: week.semana_epidemiologica,
    estimados: week.casos_est,
    notificados: week.casos_notificados,
  }))

  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-card sm:p-6" aria-labelledby="chart-title">
      <div>
        <h2 id="chart-title" className="text-base font-semibold text-slate-950">Evolução dos casos</h2>
        <p className="mt-1 text-sm text-slate-500">Comparativo das três últimas semanas disponíveis</p>
      </div>

      <div className="mt-6 h-72 w-full" aria-label="Gráfico de casos estimados e notificados">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={chartData} margin={{ top: 4, right: 8, left: -14, bottom: 0 }}>
            <CartesianGrid strokeDasharray="4 4" stroke="#e2e8f0" vertical={false} />
            <XAxis dataKey="semana" tick={{ fill: '#64748b', fontSize: 12 }} tickLine={false} axisLine={false} dy={10} />
            <YAxis tick={{ fill: '#64748b', fontSize: 12 }} tickLine={false} axisLine={false} />
            <Tooltip content={<ChartTooltip />} />
            <Legend wrapperStyle={{ paddingTop: 20, fontSize: 12 }} />
            <Line name="Estimados" type="monotone" dataKey="estimados" stroke="#0284c7" strokeWidth={3} dot={{ r: 4, fill: '#0284c7' }} activeDot={{ r: 6 }} />
            <Line name="Notificados" type="monotone" dataKey="notificados" stroke="#6366f1" strokeWidth={3} dot={{ r: 4, fill: '#6366f1' }} activeDot={{ r: 6 }} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </section>
  )
}
