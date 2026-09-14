import { beforeEach, describe, expect, it, vi } from 'vitest'
import { fetchDengueWeek, fetchLatestWeeks } from '../api/dengueApi.js'
import { loadDengueDashboardData } from './dengueDashboardService.js'

vi.mock('../api/dengueApi.js', () => ({
  fetchLatestWeeks: vi.fn(),
  fetchDengueWeek: vi.fn(),
}))

describe('loadDengueDashboardData', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('performs one weekly request for each of the three latest weeks', async () => {
    const weeks = [
      { ey: 2026, ew: 35 },
      { ey: 2026, ew: 34 },
      { ey: 2026, ew: 33 },
    ]
    fetchLatestWeeks.mockResolvedValue(weeks)
    fetchDengueWeek.mockImplementation((week) => Promise.resolve(week))

    const result = await loadDengueDashboardData()

    expect(fetchLatestWeeks).toHaveBeenCalledWith(3, undefined)
    expect(fetchDengueWeek).toHaveBeenCalledTimes(3)
    expect(fetchDengueWeek).toHaveBeenNthCalledWith(1, weeks[0], undefined)
    expect(fetchDengueWeek).toHaveBeenNthCalledWith(2, weeks[1], undefined)
    expect(fetchDengueWeek).toHaveBeenNthCalledWith(3, weeks[2], undefined)
    expect(result).toEqual(weeks)
  })

  it('rejects incomplete data sets', async () => {
    fetchLatestWeeks.mockResolvedValue([{ ey: 2026, ew: 35 }])

    await expect(loadDengueDashboardData()).rejects.toThrow(
      'Ainda não existem três semanas de dados no banco.',
    )
    expect(fetchDengueWeek).not.toHaveBeenCalled()
  })
})
