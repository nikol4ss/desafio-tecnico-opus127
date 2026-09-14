import { fetchDengueWeek, fetchLatestWeeks } from '../api/dengueApi.js'

const REQUIRED_WEEK_COUNT = 3

export async function loadDengueDashboardData(signal) {
  const weeks = await fetchLatestWeeks(REQUIRED_WEEK_COUNT, signal)

  if (!Array.isArray(weeks) || weeks.length !== REQUIRED_WEEK_COUNT) {
    throw new Error('Ainda não existem três semanas de dados no banco.')
  }

  return Promise.all(weeks.map((week) => fetchDengueWeek(week, signal)))
}
