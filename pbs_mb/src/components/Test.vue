<template>
  <div>
    <x-header :left-options="{showBack: true, backText: '返回'}">查看最近7天的记录</x-header>
    <br>
    <div>
       
      <x-button type="primary" @click.native="test"  >test</x-button>
      <x-img :src="xcImgSrc" class="preview"></x-img>
      <img src="/storage/emulated/0/BaiduShotPictures/BaiduShot_1525855019331.jpg"  id="showImg" width="128" height="128">
    </div>
  </div>
</template>

<script>
  import { XHeader, XButton, XImg } from 'vux'

  export default {
    components: {      
      XHeader,
      XButton,
      XImg
    },
    methods: {
      test () {
        Cordova.exec(this.onSuccess, this.onError, 'Interactive', 'shotMapInfo', [])

      },
      onSuccess: function (result) {
        alert('ok:' + result)
        var res = JSON.parse(result)
        this.xcImgSrc = res.shot_picture_path
        alert(this.xcImgSrc)
        var image = document.getElementById('showImg')
        image.src = res.shot_picture_path
        //image.src = event.target.result
        //alert(res.shot_picture_path)
      },

      onError: function (result) {
        alert('error' + result)
      }
    },
    data () {
      return {
        xcImgSrc: '/storage/emulated/0/BaiduShotPictures/BaiduShot_1525855019331.jpg',
        status: {
          pulldownStatus: 'default'
        }
      }
    }
  }
</script>
<style>
  .uploadbutton{
  }
  .preview {
    width: 200px;
    height: 200px
  }
  .demo5-item {
    width: 100px;
    height: 26px;
    line-height: 26px;
    text-align: center;
    border-radius: 3px;
    border: 1px solid #ccc;
    background-color: #fff;
    margin-right: 6px;
  }
  .demo5-item-selected {
    background: #ffffff url(../assets/active.png) no-repeat right bottom;
    border-color: #ff4a00;
  }
</style>