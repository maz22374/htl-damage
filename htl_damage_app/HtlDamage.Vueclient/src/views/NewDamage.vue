<script setup>
import axios from 'axios';
import webuntis from '../services/WebuntisClient.js';
import ImageRecorder from '../components/ImageRecorder.vue';
</script>

<template>
    <div class="newDamageView">
        <div>
            <h5>Stunden im Raum C3.10 aus WebUntis (Fakedaten)</h5>
            <ul>
                <li v-for="l in lessons" v-bind:key="l.lesson">Lesson {{ l.lesson }}: {{ l.classes }}</li>
            </ul>
        </div>
        <div>
            <h5>Image Upload</h5>
            <p>(Demo mit dem HTTP Upload, die Component kommt vom anderen Team)</p>
            <image-recorder v-on:recorded="uploadImage"></image-recorder>
            <div v-if="dataUrl">
                <pre>{{ dataUrl }}</pre>
                <img style="width: 300px" v-bind:src="dataUrl" />
            </div>
        </div>
    </div>
</template>

<style scoped>
.newDamageView {
    display: flex;
    flex-direction: column;
    gap:2em;
}
</style>

<script>
export default {
    components: { ImageRecorder },
    setup() {},
    data() {
        return {
            dataUrl: '',
            lessons: [],
        };
    },
    async mounted() {
        await webuntis.login('xxx', 'yyy');
        this.lessons = await webuntis.getLessonsInRoom(new Date('2023-01-30'), 'C3.10');
    },
    methods: {
        uploadImage(dataUrl) {
            this.dataUrl = dataUrl;
        },
    },
    computed: {},
};
</script>