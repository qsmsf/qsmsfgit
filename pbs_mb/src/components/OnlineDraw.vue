<template>  
  <div class="inner draw" @mousemove="beginPath($event)">
    <x-header :left-options="{showBack: true, backText: '返回'}">签名</x-header>
    <canvas 
      id="canvas" 
      class="fl" 
      width="400"
      height="400" 
      @mousedown="canvasDown($event)" 
      @mouseup="canvasUp($event)"
      @mousemove="canvasMove($event)"
      @touchstart="canvasDown($event)" 
      @touchend="canvasUp($event)"
      @touchmove="canvasMove($event)"
    >
    </canvas>
    <flexbox>
      <flexbox-item>
        <x-button type="default" @click.native="prevAction">前一步</x-button>
      </flexbox-item>
      <flexbox-item>
        <x-button type="warn" @click.native="clearRect">清空</x-button>
      </flexbox-item>
      <flexbox-item>
        <x-button type="primary" @click.native="nextAction">后一步</x-button>
      </flexbox-item>
    </flexbox>
    <x-button type="primary" @click.native="savePic" :disabled="disableSave">保存图片</x-button>
    <confirm v-model="show" title="提交" @on-confirm="onConfirm" >
      <p style="text-align:center;">确认提交并上传签名?</p>
    </confirm>
  </div>
</template>

<script>
import { XButton, XHeader, Flexbox, FlexboxItem, Divider, Confirm } from 'vux'
import { SET_QMBASE64 } from '../mutationTypes'
  export default {
    components: {
      XButton,
      XHeader,
      Flexbox,
      FlexboxItem,
      Divider,
      Confirm
    },
    data () {
      return {
        colors: ['#fef4ac', '#0018ba', '#ffc200', '#f32f15', '#cccccc', '#5ab639'],
        brushs: [{
          className: 'small fa fa-paint-brush',
          lineWidth: 3
        }, {
          className: 'middle fa fa-paint-brush',
          lineWidth: 6
        }, {
          className: 'big fa fa-paint-brush',
          lineWidth: 12
        }],
        context: {},
        imgUrl: [],
        canvasMoveUse: true,
        // 存储当前表面状态数组-上一步
        preDrawAry: [],
        // 存储当前表面状态数组-下一步
        nextDrawAry: [],
        // 中间数组
        middleAry: [],
        // 配置参数
        config: {
          lineWidth: 1,
          lineColor: '#000000FF',
          shadowBlur: 1
        },
        disableSave: false,
        show: false,
        isAdd: true
      }
    },
    mounted () {
      if (this.$store.getters.GetterToken === '') {
        this.$router.push({path: '/'})
      }
      this.isAdd = this.$route.query.isAdd
      const canvas = document.querySelector('#canvas')
      this.context = canvas.getContext('2d')
      this.initDraw()
      this.setCanvasStyle()      
    },
    destroyed () {
    },    
    methods: {
      isPc () {
        const userAgentInfo = navigator.userAgent
        const Agents = ['Android', 'iPhone', 'SymbianOS', 'Windows Phone', 'iPad', 'iPod']
        let flag = true
        for (let v = 0; v < Agents.length; v++) {
          if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false
            break
          }
        }
        return flag
      },
      initDraw () {
        const preData = this.context.getImageData(0, 0, 400, 400)
        // 空绘图表面进栈
        this.middleAry.push(preData)
      },
      canvasMove (e) {
        if (this.canvasMoveUse) {
          //console.log('canvasMove')
          const t = e.target
          let canvasX
          let canvasY
          if (this.isPc()) {
            canvasX = e.clientX - t.parentNode.offsetLeft
            canvasY = e.clientY - t.parentNode.offsetTop - 50
          } else {
            canvasX = e.changedTouches[0].clientX - t.parentNode.offsetLeft
            canvasY = e.changedTouches[0].clientY - t.parentNode.offsetTop - 70
          }
          this.context.lineTo(canvasX, canvasY)
          this.context.stroke()
        }
      },
      beginPath (e) {
        const canvas = document.querySelector('#canvas')
        if (e.target !== canvas) {
          //console.log('beginPath')
          this.context.beginPath()
        }
      },
      // mouseup
      canvasUp (e) {
        //console.log('canvasUp')
        const preData = this.context.getImageData(0, 0, 400, 400)
        if (!this.nextDrawAry.length) {
          // 当前绘图表面进栈
          this.middleAry.push(preData)
        } else {
          this.middleAry = []
          this.middleAry = this.middleAry.concat(this.preDrawAry)
          this.middleAry.push(preData)
          this.nextDrawAry = []
        }
        this.canvasMoveUse = false
      },
      // mousedown
      canvasDown (e) {
        //console.log('canvasDown')
        this.canvasMoveUse = true
        // client是基于整个页面的坐标
        // offset是cavas距离顶部以及左边的距离
        const canvasX = e.clientX - e.target.parentNode.offsetLeft
        const canvasY = e.clientY - e.target.parentNode.offsetTop - 50
        this.setCanvasStyle()
        // 清除子路径
        this.context.beginPath()
        this.context.moveTo(canvasX, canvasY)
        //console.log('moveTo', canvasX, canvasY)
        // 当前绘图表面状态
        const preData = this.context.getImageData(0, 0, 400, 400)
        // 当前绘图表面进栈
        this.preDrawAry.push(preData)
      },
      // 设置颜色
      setColor (color) {
        this.config.lineColor = color
      },
      // 设置笔刷大小
      setBrush (type) {
        this.config.lineWidth = type
      },
      clearRect () {
        this.context.clearRect(0, 0, this.context.canvas.width, this.context.canvas.height)
            this.preDrawAry = []
            this.nextDrawAry = []
            this.middleAry = [this.middleAry[0]]
      },
      prevAction () {
        if (this.preDrawAry.length) {
          const popData = this.preDrawAry.pop()
          const midData = this.middleAry[this.preDrawAry.length + 1]
          this.nextDrawAry.push(midData)
          this.context.putImageData(popData, 0, 0)
        }
      },
      nextAction () {
        if (this.nextDrawAry.length) {
          const popData = this.nextDrawAry.pop()
          const midData = this.middleAry[this.middleAry.length - this.nextDrawAry.length - 2]
          this.preDrawAry.push(midData)
          this.context.putImageData(popData, 0, 0)
        }
      },
      dataURItoBlob: function (base64Data) {  
        var byteString;  
        if (base64Data.split(',')[0].indexOf('base64') >= 0)  
            byteString = atob(base64Data.split(',')[1])
        else  
            byteString = unescape(base64Data.split(',')[1])
        var mimeString = base64Data.split(',')[0].split(':')[1].split(';')[0] 
        var ia = new Uint8Array(byteString.length)
        for (var i = 0; i < byteString.length; i++) {  
            ia[i] = byteString.charCodeAt(i)
        }  
        return new Blob([ia], {type: mimeString})
      },
      // 操作
      savePic () {
        this.show = true
      },
      onConfirm () {
        const canvas = document.querySelector('#canvas')
        var src = canvas.toDataURL('image/png')
        
        this.$store.commit(SET_QMBASE64, src)
        var blob = this.dataURItoBlob(src)
      
        //var req = new XMLHttpRequest()

        //req.open("POST", this.$store.getters.GetterBaseUrl + 'api/Files/singleUpload')

        //req.send(form)
        var fileName = this.$store.getters.GetterEntity.uuid + '.png'
        var fd = new FormData();

        fd.append(fileName, blob);//fileData为自定义    
        console.log(blob)
        if(isProxy){
          let headers = {
            token: this.$store.getters.GetterToken
          }
          let headerdata = JSON.stringify(headers)
          
          xh.postFile(_url + '/api/Files/singleUpload',
          {
            headerParameter: headerdata,
            bodyEntity: fd
          },
          function(res){
            if (res.data.code === 100) {
              this.recList = res.data.resultList
              this.$refs.recordScroller.donePulldown()
            } else {
              this.$vux.toast.show({
                text: res.data.message,
                type: 'warn'
              })
              this.$refs.recordScroller.donePulldown()
            }
          }, 
          function (res) {
            this.$vux.toast.show({
              text: err,
              type: 'warn'
            })
            this.$refs.recordScroller.donePulldown()
          })        
        }else{
          this.$http({
            url: this.$store.getters.GetterBaseUrl + 'api/Files/singleUpload',
            method: 'POST',
            body: fd,
            headers: {
              contentType: 'application/x-www-form-urlencoded',
              token: this.$store.getters.GetterToken
            }
          }).then(function (res) {
            if (res.status === 200) {

              this.$router.push({name: 'Step4',query: {isAdd: this.isAdd, curStep: 4}})
              //this.success()
            } else {
              this.$vux.toast.show({
                text: res.data.message,
                type: 'warn'
              })
              this.disableSave = false
            }
          }).catch(err => {
            this.$vux.toast.show({
              text: err,
              type: 'warn'
            })
            this.disableSave = false
          })
        }
      },
      // 设置绘画配置
      setCanvasStyle () {
        this.context.lineWidth = this.config.lineWidth
        this.context.shadowBlur = this.config.shadowBlur
        this.context.shadowColor = this.config.lineColor
        this.context.strokeStyle = this.config.lineColor
      }
    }
  }
</script>

<style>
  
  .inner.draw {
    
  }
  .draw h5 {
    margin-bottom: 10px;
  }
  #img-box {
    flex: 1;
    padding-left: 10px;
  }
  #img-box .img-item {
    position: relative;
    display: inline-block;
  }
  #img-box .img-item .fa {
    position: absolute;
    cursor: pointer;
    right: 1px;
    top: -1px;
    font-size: 12px;
    font-weight: 1;
    display: none;
    color: #ccc;
  }
  #img-box .img-item:hover .fa {
    display: block;
  }
  #img-box .img-item .fa:hover {
    color: #f2849e;
  }
  #img-box img {
    border: 1px #ccc solid;
    width: 90px;
    height: 60px;
    margin: 5px;
  }
  .wrap{
    width: 100%;
    border: 1px #585858 solid;
    overflow: hidden;
  }
  .fl{
    float: left;
    display: block;
  }
  #canvas{
    border-right: 1px #585858 solid;
    cursor: crosshair;
  }
  #control{
    width: 130px;
    height: 400px;
    margin-left: 4px;
  }
  #control div{
    padding: 5px;
  }
  #canvas-color ul{
    overflow: hidden;
    margin: 0;
    padding: 0;
  }
  #canvas-color ul li{
    float: left;
    display: inherit;
    width: 13px;
    height: 13px;
    border: 3px #fff solid;
    margin: 8px;
    cursor: pointer;
  }
  #canvas-color .active {
    border: 1px solid #f2849e;
  }
  #canvas-brush span{
    display: inline-block;
    width: 20px;
    height: 15px;
    margin-left: 10px;
    cursor: pointer;
  }
  #canvas-brush .small {
    font-size: 12px;
  }
  #canvas-brush .middle {
    font-size: 14px;
  }
  #canvas-brush .big {
    font-size: 16px;
  }
 
  #canvas-control span{
    display: inline-block;
    font-size: 14px;
    width: 20px;
    height: 15px;
    margin-left: 10px;
    cursor: pointer;
  }
  #canvas-control .active,
  #canvas-brush .active {
    color: #f2849e;
  }
  .drawImage {
    width: 100px;
    height: 30px;
    font-size: 12px;
    line-height: 30px;
  }
</style>