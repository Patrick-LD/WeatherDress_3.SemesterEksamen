<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M4 34 Q4 10 32 8 Q60 10 60 34 Q54 30 50 34 Q44 30 40 34 Q36 30 32 34 Q28 30 24 34 Q20 30 14 34 Q10 30 4 34 Z"
          fill="#c0392b" stroke="#7a1f15" stroke-width="2" stroke-linejoin="round"/>
    <path d="M4 34 Q10 30 14 34 Q20 30 24 34 Q28 30 32 34 Q36 30 40 34 Q44 30 50 34 Q54 30 60 34" fill="none" stroke="#7a1f15" stroke-width="1.5"/>
    <path d="M14 22 Q14 14 32 10 M50 22 Q50 14 32 10 M32 10 L32 34" stroke="#7a1f15" stroke-width="1.2" fill="none"/>
    <line x1="32" y1="34" x2="32" y2="54" stroke="#3a2418" stroke-width="2.5"/>
    <path d="M32 54 Q32 60 38 60" fill="none" stroke="#3a2418" stroke-width="2.5" stroke-linecap="round"/>
    <circle cx="32" cy="8" r="2" fill="#7a1f15"/>
    <line x1="8" y1="48" x2="6" y2="56" stroke="#4a90e2" stroke-width="2" stroke-linecap="round"/>
    <line x1="20" y1="50" x2="18" y2="58" stroke="#4a90e2" stroke-width="2" stroke-linecap="round"/>
    <line x1="48" y1="48" x2="46" y2="56" stroke="#4a90e2" stroke-width="2" stroke-linecap="round"/>
    <line x1="56" y1="50" x2="54" y2="58" stroke="#4a90e2" stroke-width="2" stroke-linecap="round"/>
  </svg>
</template>
