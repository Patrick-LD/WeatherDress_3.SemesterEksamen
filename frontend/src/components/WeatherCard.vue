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
  iconSize: { type: Number, default: 64 },
  items: { type: Array, default: () => [] }
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
    <div :class="items.length ? 'card-inner' : ''">
      <div class="weather-info">
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

      <div v-if="items.length" class="clothing-panel">
        <p class="clothing-title">Hvad skal du have på?</p>
        <ul class="clothing-panel-list">
          <li v-for="item in items" :key="item" class="clothing-panel-item">{{ item }}</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.card-inner {
  display: flex;
  gap: 24px;
  align-items: flex-start;
}

.weather-info {
  flex: 1;
}

.clothing-panel {
  flex: 1;
  border-left: 2px solid #eaf4fd;
  padding-left: 24px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  min-height: 100%;
}

.clothing-title {
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #1a6da8;
  margin-bottom: 16px;
}

.clothing-panel-list {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.clothing-panel-item {
  font-size: 1rem;
  color: #1a1a2e;
  padding: 8px 16px;
  background: #eaf4fd;
  border-radius: 10px;
  font-weight: 500;
}
</style>
