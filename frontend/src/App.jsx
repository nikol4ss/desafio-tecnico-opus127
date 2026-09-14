import { CasesChart } from './components/CasesChart.jsx'
import { DashboardHeader } from './components/DashboardHeader.jsx'
import { ErrorState, LoadingState } from './components/DashboardStates.jsx'
import { SummaryCard } from './components/SummaryCard.jsx'
import { WeekCard } from './components/WeekCard.jsx'
import { WeeklyTable } from './components/WeeklyTable.jsx'
import { useDengueData } from './hooks/useDengueData.js'
import { formatNumber } from './utils/formatters.js'

export default function App() {
  const { data, error, isLoading, reload } = useDengueData()
  const totalEstimated = data.reduce((total, week) => total + week.casos_est, 0)
  const totalNotified = data.reduce((total, week) => total + week.casos_notificados, 0)

  return (
    <div className="min-h-screen">
      <DashboardHeader />

      <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div className="mb-8">
          <p className="text-sm font-semibold text-brand-700">Belo Horizonte · MG</p>
          <h1 className="mt-2 text-3xl font-bold tracking-tight text-slate-950 sm:text-4xl">Panorama da dengue</h1>
          <p className="mt-3 max-w-2xl text-sm leading-6 text-slate-600 sm:text-base">
            Acompanhe os casos estimados, as notificações e o nível de alerta nas três semanas epidemiológicas mais recentes.
          </p>
        </div>

        {isLoading && <LoadingState />}
        {!isLoading && error && <ErrorState message={error} onRetry={reload} />}

        {!isLoading && !error && data.length > 0 && (
          <div className="space-y-6">
            <section className="grid gap-4 sm:grid-cols-3" aria-label="Resumo do período">
              <SummaryCard label="Casos estimados" value={formatNumber(totalEstimated)} detail="Soma das três semanas" icon="estimated" />
              <SummaryCard label="Casos notificados" value={formatNumber(totalNotified)} detail="Soma das três semanas" icon="notified" />
              <SummaryCard label="Período analisado" value="3 semanas" detail={`Até a SE ${data[0].semana_epidemiologica}`} icon="period" />
            </section>

            <section aria-labelledby="weeks-title">
              <div className="mb-4 flex items-end justify-between gap-4">
                <div>
                  <h2 id="weeks-title" className="text-lg font-semibold text-slate-950">Semanas recentes</h2>
                  <p className="mt-1 text-sm text-slate-500">Informações obtidas do banco de dados local</p>
                </div>
              </div>
              <div className="grid gap-4 lg:grid-cols-3">
                {data.map((week, index) => (
                  <WeekCard key={week.semana_epidemiologica} week={week} isLatest={index === 0} />
                ))}
              </div>
            </section>

            <div className="grid gap-6 xl:grid-cols-[1.05fr_0.95fr]">
              <CasesChart data={data} />
              <WeeklyTable data={data} />
            </div>

            <p className="text-center text-xs leading-5 text-slate-500">
              Fonte: InfoDengue. Os valores estimados podem ser atualizados retrospectivamente a cada semana.
            </p>
          </div>
        )}
      </main>
    </div>
  )
}
