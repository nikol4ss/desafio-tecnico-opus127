import { AlertBadge } from './AlertBadge.jsx'
import { formatDate, formatNumber } from '../utils/formatters.js'

export function WeeklyTable({ data }) {
  return (
    <section className="hidden overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-card md:block" aria-labelledby="table-title">
      <div className="border-b border-slate-200 px-5 py-5 sm:px-6">
        <h2 id="table-title" className="text-base font-semibold text-slate-950">Detalhamento semanal</h2>
        <p className="mt-1 text-sm text-slate-500">Dados consolidados por semana epidemiológica</p>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-slate-200 text-left">
          <thead className="bg-slate-50">
            <tr>
              <th scope="col" className="px-5 py-3 text-xs font-semibold uppercase tracking-wide text-slate-500 sm:px-6">Semana</th>
              <th scope="col" className="px-5 py-3 text-xs font-semibold uppercase tracking-wide text-slate-500">Casos estimados</th>
              <th scope="col" className="px-5 py-3 text-xs font-semibold uppercase tracking-wide text-slate-500">Casos notificados</th>
              <th scope="col" className="px-5 py-3 text-xs font-semibold uppercase tracking-wide text-slate-500">Nível de alerta</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {data.map((week) => (
              <tr key={week.semana_epidemiologica} className="transition-colors hover:bg-slate-50/70">
                <td className="whitespace-nowrap px-5 py-4 sm:px-6">
                  <p className="text-sm font-semibold text-slate-900">{week.semana_epidemiologica}</p>
                  <p className="mt-0.5 text-xs text-slate-500">{formatDate(week.data_inicio)}</p>
                </td>
                <td className="whitespace-nowrap px-5 py-4 text-sm font-medium text-slate-700">{formatNumber(week.casos_est)}</td>
                <td className="whitespace-nowrap px-5 py-4 text-sm font-medium text-slate-700">{formatNumber(week.casos_notificados)}</td>
                <td className="whitespace-nowrap px-5 py-4"><AlertBadge level={week.nivel_alerta} /></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  )
}
