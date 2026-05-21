<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <rect x="8" y="46" width="48" height="8" rx="4" fill="#1a1a2e"/>
    <path d="M10 46 L10 34 Q10 26 18 24 L42 22 Q52 22 54 30 L56 46 Z"
          fill="#4a90e2" stroke="#2a5fa8" stroke-width="1.5" stroke-linejoin="round"/>
    <path d="M10 46 L10 36 Q10 30 16 28 L22 27" fill="none" stroke="white" stroke-width="1.5" opacity="0.4"/>
    <path d="M26 26 L30 40 M36 24 L38 40" stroke="white" stroke-width="1" opacity="0.5"/>
    <path d="M26 30 L38 28 M28 35 L40 33 M29 40 L41 38" stroke="white" stroke-width="1" opacity="0.5"/>
  </svg>
</template>
