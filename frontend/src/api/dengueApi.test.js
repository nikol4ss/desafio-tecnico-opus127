import { afterEach, describe, expect, it, vi } from 'vitest'
import { fetchDengueWeek, fetchLatestWeeks } from './dengueApi.js'

describe('dengueApi', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
  })

  it('requests the latest week identifiers', async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve([]),
    })
    vi.stubGlobal('fetch', fetchMock)

    await fetchLatestWeeks(3)

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/dengue/weeks/latest?count=3',
      expect.objectContaining({ headers: { Accept: 'application/json' } }),
    )
  })

  it('uses ew and ey when requesting a week', async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({}),
    })
    vi.stubGlobal('fetch', fetchMock)

    await fetchDengueWeek({ ew: 35, ey: 2026 })

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/dengue?ew=35&ey=2026',
      expect.objectContaining({ headers: { Accept: 'application/json' } }),
    )
  })
})
