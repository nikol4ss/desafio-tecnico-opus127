const numberFormatter = new Intl.NumberFormat('pt-BR', {
  maximumFractionDigits: 1,
})

const dateFormatter = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  timeZone: 'UTC',
})

export function formatNumber(value) {
  return numberFormatter.format(value)
}

export function formatDate(value) {
  return dateFormatter.format(new Date(`${value}T00:00:00Z`)).replace('.', '')
}
