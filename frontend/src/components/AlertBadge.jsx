import { getAlertLevel } from '../utils/alertLevels.js'

export function AlertBadge({ level, showDescription = false }) {
  const alert = getAlertLevel(level)

  return (
    <span
      className={`inline-flex items-center gap-2 rounded-full px-2.5 py-1 text-xs font-semibold ring-1 ring-inset ${alert.badgeClass}`}
      aria-label={`Nível ${level}: ${alert.label}`}
    >
      <span className={`size-1.5 rounded-full ${alert.dotClass}`} aria-hidden="true" />
      {alert.label}
      {showDescription && <span className="font-normal opacity-75">· {alert.description}</span>}
    </span>
  )
}
