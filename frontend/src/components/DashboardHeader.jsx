export function DashboardHeader() {
  return (
    <header className="border-b border-slate-200/80 bg-white/80 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4 sm:px-6 lg:px-8">
        <div className="flex items-center gap-3">
          <div className="grid size-10 place-items-center rounded-xl bg-brand-600 text-white shadow-sm">
            <svg aria-hidden="true" viewBox="0 0 24 24" className="size-6" fill="none" stroke="currentColor" strokeWidth="1.8">
              <path strokeLinecap="round" strokeLinejoin="round" d="M4 16.5 8.5 12l3 3L20 6.5" />
              <path strokeLinecap="round" strokeLinejoin="round" d="M15 6.5h5v5" />
            </svg>
          </div>
          <div>
            <p className="text-sm font-semibold tracking-tight text-slate-950">Dengue BH</p>
            <p className="text-xs text-slate-500">Monitoramento epidemiológico</p>
          </div>
        </div>

        <div className="hidden items-center gap-2 text-xs font-medium text-slate-500 sm:flex">
          <span className="size-2 rounded-full bg-emerald-500" aria-hidden="true" />
          Dados oficiais InfoDengue
        </div>
      </div>
    </header>
  )
}
