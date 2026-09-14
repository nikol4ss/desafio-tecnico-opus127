export function LoadingState() {
  return (
    <div className="grid gap-4" role="status" aria-label="Carregando dados">
      <div className="h-28 animate-pulse rounded-2xl bg-slate-200/70" />
      <div className="grid gap-4 sm:grid-cols-3">
        {[0, 1, 2].map((item) => (
          <div key={item} className="h-44 animate-pulse rounded-2xl bg-slate-200/70" />
        ))}
      </div>
      <span className="sr-only">Carregando dados epidemiológicos...</span>
    </div>
  )
}

export function ErrorState({ message, onRetry }) {
  return (
    <div className="rounded-2xl border border-red-200 bg-white px-6 py-12 text-center shadow-card" role="alert">
      <div className="mx-auto grid size-12 place-items-center rounded-full bg-red-50 text-red-600">
        <svg aria-hidden="true" viewBox="0 0 24 24" className="size-6" fill="none" stroke="currentColor" strokeWidth="1.8">
          <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v4m0 4h.01M10.3 4.1 2.7 17.25A1.75 1.75 0 0 0 4.2 19.9h15.6a1.75 1.75 0 0 0 1.5-2.65L13.7 4.1a1.97 1.97 0 0 0-3.4 0Z" />
        </svg>
      </div>
      <h2 className="mt-4 text-lg font-semibold text-slate-950">Não foi possível carregar o painel</h2>
      <p className="mx-auto mt-2 max-w-lg text-sm text-slate-600">{message}</p>
      <button type="button" onClick={onRetry} className="mt-6 rounded-xl bg-brand-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-brand-700">
        Tentar novamente
      </button>
    </div>
  )
}
