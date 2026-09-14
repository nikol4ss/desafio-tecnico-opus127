const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

async function getJson(path, signal) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { Accept: 'application/json' },
    signal,
  })

  if (!response.ok) {
    let detail = 'Não foi possível carregar os dados.'

    try {
      const problem = await response.json()
      detail = problem.detail ?? problem.title ?? detail
    } catch {
      // Keep the stable user-facing message when the response is not JSON.
    }

    throw new Error(detail)
  }

  return response.json()
}

export function fetchLatestWeeks(count = 3, signal) {
  return getJson(`/dengue/weeks/latest?count=${count}`, signal)
}

export function fetchDengueWeek({ ey, ew }, signal) {
  return getJson(`/dengue?ew=${ew}&ey=${ey}`, signal)
}
