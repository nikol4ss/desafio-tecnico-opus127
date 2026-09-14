import { useCallback, useEffect, useState } from 'react'
import { loadDengueDashboardData } from '../services/dengueDashboardService.js'

export function useDengueData() {
  const [state, setState] = useState({
    data: [],
    error: null,
    isLoading: true,
  })
  const [reloadKey, setReloadKey] = useState(0)

  const reload = useCallback(() => setReloadKey((key) => key + 1), [])

  useEffect(() => {
    const controller = new AbortController()

    async function load() {
      setState((current) => ({ ...current, error: null, isLoading: true }))

      try {
        const data = await loadDengueDashboardData(controller.signal)
        setState({ data, error: null, isLoading: false })
      } catch (error) {
        if (error.name !== 'AbortError') {
          setState({
            data: [],
            error: error instanceof Error ? error.message : 'Erro inesperado.',
            isLoading: false,
          })
        }
      }
    }

    load()
    return () => controller.abort()
  }, [reloadKey])

  return { ...state, reload }
}
