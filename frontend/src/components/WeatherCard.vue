<script setup>
import { computed } from 'vue'
import { capitalize } from '../utils/format.js'
import SolIcon from './icons/SolIcon.vue'
import RegnIcon from './icons/RegnIcon.vue'
import SkyetIcon from './icons/SkyetIcon.vue'
import SneIcon from './icons/SneIcon.vue'

const props = defineProps({
  title: { type: String, required: true },
  data: { type: Object, required: true },
  sectionId: { type: String, required: true },
  isToday: { type: Boolean, default: false },
  iconSize: { type: Number, default: 64 }
})

const weatherIcon = computed(() => {
  const desc = props.data.description?.toLowerCase() ?? ''
  if (desc.includes('sne')) return SneIcon
  if (desc.includes('regn') || desc.includes('støvregn') || desc.includes('torden')) return RegnIcon
  if (desc.includes('skyet') || desc.includes('overskyet') || desc.includes('tåge')) return SkyetIcon
  if (desc.includes('klar') || desc.includes('sol')) return SolIcon
  return null
})
</script>

<template>
  <div class="card" :id="sectionId">
    <h2>{{ title }}</h2>
    <component :is="weatherIcon" v-if="weatherIcon" :size="iconSize" class="weather-icon" />
    <div class="big-temp" :id="isToday ? 'temperatur' : null">
      {{ Math.round(data.temperatureC) }}<span>°C</span>
    </div>
    <div class="badge" :id="isToday ? 'vejr-beskrivelse' : null">
      {{ capitalize(data.description) }}
    </div>
    <div class="weather-detail">
      <span class="label">Fugtighed</span>
      <strong :id="isToday ? 'fugtighed' : null">{{ data.humidity }} %</strong>
    </div>
    <div class="weather-detail">
      <span class="label">Vind</span>
      <strong>{{ data.windSpeed }} km/t</strong>
    </div>
    <div class="weather-detail">
      <span class="label">Nedbør</span>
      <strong>{{ data.precipitation }} mm</strong>
    </div>
  </div>
</template>
