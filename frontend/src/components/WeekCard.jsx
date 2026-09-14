import { AlertBadge } from './AlertBadge.jsx'
import { formatDate, formatNumber } from '../utils/formatters.js'

export function WeekCard({ week, isLatest }) {
  return (
    <article className={`rounded-2xl border bg-white p-5 shadow-card ${isLatest ? 'border-brand-200 ring-2 ring-brand-100' : 'border-slate-200'}`}>
      <div className="flex items-start justify-between gap-3">
        <div>
          <div className="flex items-center gap-2">
            <h3 className="font-semibold text-slate-950">SE {week.semana_epidemiologica}</h3>
            {isLatest && (
              <span className="rounded-md bg-brand-50 px-1.5 py-0.5 text-[10px] font-bold uppercase tracking-wide text-brand-700">
                Mais recente
              </span>
            )}
          </div>
          <p className="mt-1 text-xs text-slate-500">Início em {formatDate(week.data_inicio)}</p>
        </div>
        <AlertBadge level={week.nivel_alerta} />
      </div>

      <dl className="mt-5 grid grid-cols-2 gap-3 border-t border-slate-100 pt-4">
        <div>
          <dt className="text-xs text-slate-500">Casos estimados</dt>
          <dd className="mt-1 text-xl font-bold text-slate-900">{formatNumber(week.casos_est)}</dd>
        </div>
        <div>
          <dt className="text-xs text-slate-500">Casos notificados</dt>
          <dd className="mt-1 text-xl font-bold text-slate-900">{formatNumber(week.casos_notificados)}</dd>
        </div>
      </dl>
    </article>
  )
}
