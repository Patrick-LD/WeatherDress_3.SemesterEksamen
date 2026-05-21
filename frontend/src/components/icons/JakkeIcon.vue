<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M20 8 L8 14 L4 36 L14 40 L16 30 L16 58 L48 58 L48 30 L50 40 L60 36 L56 14 L44 8 L38 8 L32 16 L26 8 Z"
          fill="#3a6a9a" stroke="#0f2d4a" stroke-width="2" stroke-linejoin="round"/>
    <path d="M26 8 L20 18 L32 16 L44 18 L38 8" fill="#2d5278" stroke="#0f2d4a" stroke-width="1.8" stroke-linejoin="round"/>
    <line x1="32" y1="16" x2="32" y2="58" stroke="#0f2d4a" stroke-width="2"/>
    <circle cx="28" cy="28" r="1.6" fill="#0f2d4a"/>
    <circle cx="28" cy="38" r="1.6" fill="#0f2d4a"/>
    <circle cx="28" cy="48" r="1.6" fill="#0f2d4a"/>
  </svg>
</template>
