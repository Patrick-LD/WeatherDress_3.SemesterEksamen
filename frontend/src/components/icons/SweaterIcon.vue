<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M22 10 L8 14 L4 38 L14 40 L16 30 L16 56 L48 56 L48 30 L50 40 L60 38 L56 14 L42 10 L40 14 Q36 18 32 18 Q28 18 24 14 Z"
          fill="#c47042" stroke="#7a3a1a" stroke-width="2" stroke-linejoin="round"/>
    <path d="M24 10 Q28 16 32 16 Q36 16 40 10" fill="none" stroke="#7a3a1a" stroke-width="2"/>
    <path d="M18 24 L46 24 M18 32 L46 32 M18 40 L46 40 M18 48 L46 48" stroke="#7a3a1a" stroke-width="0.8" stroke-dasharray="2 2"/>
    <path d="M22 56 L22 60 L26 60 M30 56 L30 60 L34 60 M38 56 L38 60 L42 60" fill="none" stroke="#7a3a1a" stroke-width="1.2"/>
  </svg>
</template>
