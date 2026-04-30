import { ref, computed } from 'vue'
import { hentVejrToday, hentVejrYesterday } from '../services/weatherApi.js'
import { findNuværendeTime, findMiddag } from '../utils/format.js'
import { beregnTøj } from '../utils/clothing.js'
import { lavMeddelelse } from '../utils/weatherMessage.js'

export function useWeather() {
  const postnummer = ref('')
  const loading = ref(false)
  const fejl = ref('')
  const visPanel = ref(false)
  const todayData = ref([])
  const yesterdayData = ref([])

  const nuværende = computed(() => findNuværendeTime(todayData.value))
  const gårMiddag = computed(() => findMiddag(yesterdayData.value))
  const tøjListe = computed(() => beregnTøj(nuværende.value))
  const meddelelse = computed(() => lavMeddelelse(nuværende.value))

  async function søgVejr() {
    const zip = postnummer.value.trim()
    if (!zip) return

    loading.value = true
    fejl.value = ''

    try {
      const [today, yesterday] = await Promise.all([
        hentVejrToday(zip),
        hentVejrYesterday(zip)
      ])
      todayData.value = today
      yesterdayData.value = yesterday
      visPanel.value = true
    } catch (err) {
      fejl.value = err.message
    } finally {
      loading.value = false
    }
  }

  function visForside() {
    visPanel.value = false
    postnummer.value = ''
    fejl.value = ''
  }

  return {
    postnummer,
    loading,
    fejl,
    visPanel,
    nuværende,
    gårMiddag,
    tøjListe,
    meddelelse,
    søgVejr,
    visForside
  }
}
