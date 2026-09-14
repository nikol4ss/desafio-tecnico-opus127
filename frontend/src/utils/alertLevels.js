const levels = {
  1: {
    label: 'Verde',
    description: 'Baixo risco',
    badgeClass: 'bg-emerald-50 text-emerald-700 ring-emerald-600/20',
    dotClass: 'bg-emerald-500',
  },
  2: {
    label: 'Amarelo',
    description: 'Atenção',
    badgeClass: 'bg-amber-50 text-amber-700 ring-amber-600/20',
    dotClass: 'bg-amber-400',
  },
  3: {
    label: 'Laranja',
    description: 'Risco elevado',
    badgeClass: 'bg-orange-50 text-orange-700 ring-orange-600/20',
    dotClass: 'bg-orange-500',
  },
  4: {
    label: 'Vermelho',
    description: 'Alto risco',
    badgeClass: 'bg-red-50 text-red-700 ring-red-600/20',
    dotClass: 'bg-red-500',
  },
}

export function getAlertLevel(level) {
  return levels[level] ?? {
    label: 'Indefinido',
    description: 'Sem classificação',
    badgeClass: 'bg-slate-100 text-slate-600 ring-slate-500/20',
    dotClass: 'bg-slate-400',
  }
}
