<template>
    <div class="homeView">
        <h1>HTL Damage - Schäden mit Zukunft</h1>
        <div class="images">
            <div class="image">
                <img src="@/assets/images/klo.jpg" />
                <div>Scheißhaus mit Zukunft</div>
            </div>
            <div class="image">
                <img src="@/assets/images/door.jpg" />
                <div>Tür mit Zukunft</div>
            </div>
            <div class="image">
                <img src="@/assets/images/handtuchspender.jpg" />
                <div>Handtuchspender mit Zukunft</div>
            </div>
            <div class="image">
                <img src="@/assets/images/seifenspender.jpg" />
                <div>Seifenspender mit Zukunft</div>
            </div>                                    
        </div>
    </div>
</template>

<style scoped>
    .images {
        display: flex;
        flex-direction: row;
        gap: 1em;
        flex-wrap: wrap;
    }
    .image {
        flex: 0 0 200px;
    }
    img {
        width: 100%;
    }
</style>