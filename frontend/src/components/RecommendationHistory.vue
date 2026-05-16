<script setup>
import { ref, watch } from 'vue'
import { hentAnbefalingHistorik } from '../services/weatherApi.js'

const props = defineProps({
  zipCode: { type: String, required: true }
})

const historik = ref([])
const loading = ref(false)
const fejl = ref(null)

watch(() => props.zipCode, async (zip) => {
  if (!zip) return
  loading.value = true
  fejl.value = null
  try {
    historik.value = await hentAnbefalingHistorik(zip)
  } catch (e) {
    fejl.value = e.message
  } finally {
    loading.value = false
  }
}, { immediate: true })

function formatDato(dateStr) {
  const d = new Date(dateStr)
  return d.toLocaleDateString('da-DK', { weekday: 'short', day: 'numeric', month: 'short' })
}
</script>

<template>
  <div id="anbefaling-historik" class="card">
    <h2>7-dages anbefalingshistorik</h2>

    <p v-if="loading" class="historik-status">Henter historik...</p>
    <p v-else-if="fejl" class="historik-status historik-fejl">{{ fejl }}</p>
    <p v-else-if="historik.length === 0" class="historik-status">Ingen historik endnu for dette postnummer.</p>

    <div v-else class="historik-tabel-wrapper">
      <table class="historik-tabel">
        <thead>
          <tr>
            <th>Dato</th>
            <th>Temperatur</th>
            <th>Vejr</th>
            <th>Jakke</th>
            <th>Bukser</th>
            <th>Sko</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="rad in historik" :key="rad.id">
            <td>{{ formatDato(rad.date) }}</td>
            <td>{{ rad.temperatureC.toFixed(1) }} °C</td>
            <td>{{ rad.weatherDescription ?? rad.weatherCategory ?? '—' }}</td>
            <td>{{ rad.jacket ?? '—' }}</td>
            <td>{{ rad.pants ?? '—' }}</td>
            <td>
              {{ rad.shoes ?? '—' }}
              <span v-if="rad.shoesNote" class="sko-note">{{ rad.shoesNote }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
#anbefaling-historik {
  margin-top: 16px;
}

.historik-status {
  color: #666;
  font-size: 0.9rem;
  text-align: center;
  padding: 12px 0;
}

.historik-fejl {
  color: #c0392b;
}

.historik-tabel-wrapper {
  overflow-x: auto;
}

.historik-tabel {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.historik-tabel th {
  text-align: left;
  padding: 8px 12px;
  background: #eaf4fd;
  color: #1a6da8;
  font-weight: 600;
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.historik-tabel td {
  padding: 10px 12px;
  border-bottom: 1px solid #f0f4f8;
  color: #333;
}

.historik-tabel tr:last-child td {
  border-bottom: none;
}

.historik-tabel tr:hover td {
  background: #f7fbff;
}

.sko-note {
  display: block;
  font-size: 0.75rem;
  color: #888;
}
</style>
