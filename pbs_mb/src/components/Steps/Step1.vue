<template>
  <div>
    <group>
      <x-input title="案件名称" v-model="xz" text-align='right'></x-input>
      <x-input title="案件编号" v-model="record_aj_no" text-align='right'></x-input>
      <x-input title="接警编号" v-model="record_jj_no" text-align='right'></x-input>
      <x-input title="现场勘验号" v-model="record_ky_no" text-align='right'></x-input>
      <cell title="现场方位"  is-link @click.native="fetchXct" ></cell>
      <img src=""  id="xctImg" width="0" height="0">
      <cell title="现场平面"  is-link @click.native="fetchPmt" ></cell>
      <img src=""  id="pmtImg" width="0" height="0">
      <x-input title="发生区域" v-model="fsqy" text-align='right'></x-input>
      <x-input title="现场位置" v-model='xcwz' text-align='right'></x-input>      
      <popup-picker title="勘验单位" :data="unitList" v-model="kyUnit"
        @on-show="onShow" @on-hide="onHide" @on-change="onChange" show-name></popup-picker>
      <popup-picker title="接警人" :data='userList' v-model="jjr" show-name></popup-picker>
      <x-input title="其他接警人" v-model='jjrOther' text-align='right'></x-input>
      <popup-picker title="接警单位" :data="unitList" v-model="bjUnit" show-name></popup-picker>
      <datetime v-model="kyDate" title="勘验日期" confirm-text="完成" cancel-text="取消" format="YYYY-MM-DD" ></datetime>
      <datetime v-model="bjTime" title="接警时间" confirm-text="完成" cancel-text="取消" format="YYYY-MM-DD HH:mm" ></datetime>
      <datetime v-model="kyksTime" title="勘验开始时间" confirm-text="完成" cancel-text="取消" format="YYYY-MM-DD HH:mm" ></datetime>
      <datetime v-model="kyjsTime" title="勘验结束时间" placeholder="不填则默认为提交时间" confirm-text="完成" cancel-text="取消" format="YYYY-MM-DD HH:mm" ></datetime>
      <popup-picker title="天气" :data='tqList' v-model="tq" ></popup-picker>
      <popup-picker title="风向" :data='fxList' v-model="fx" ></popup-picker>
      <popup-picker title="温度" :data='wdList' v-model='wd' ></popup-picker>
      <popup-picker title="湿度" :data='sdList' v-model='sd' ></popup-picker>
      <cell title="光照" v-model="lightList" is-link @click.native="showPopup=true"></cell>
      <popup v-model="showPopup" class="checker-popup">
        <div style="padding:10px 10px 40px 10px;">
          <p style="padding: 5px 5px 5px 2px;color:#888;">光照情况</p>
          <checker
          v-model="lightList" type="checkbox"
          default-item-class="demo4-item"
          selected-item-class="demo4-item-selected"
          disabled-item-class="demo4-item-disabled">
            <checker-item value="自然光" >自然光</checker-item>
            <checker-item value="灯光" >灯光</checker-item>
            <checker-item value="特种光" >特种光</checker-item>
          </checker>
          <div style="padding: 15px;">
            <x-button @click.native="showPopup = false" plain type="primary"> 确  定 </x-button>
          </div>
        </div>
      </popup>
      <popup-picker title="现场状况" :data='zkList' v-model="xczk" ></popup-picker>
      <x-input title="变动原因" v-model='bdyy' text-align='right' v-show="showBdReason">
        <x-button slot="right" type="primary" mini @click.native="showBdyyPopup = true">></x-button>
      </x-input>
      <x-switch title="是否现场保护" v-model="bhFlag" ></x-switch>      
      <group v-if="bhFlag">
        <x-input title="保护人" v-model='bhrName' text-align='right'>
          <x-button slot="right" type="primary" mini @click.native="showBhrPopup = true">></x-button>
        </x-input>
        <popup-picker title="保护人单位" :data='unitList' v-model="bhrdw" show-name></popup-picker>
        <x-input title="保护人单位" v-model='bhrUnitName' text-align='right'>
          <x-button slot="right" type="primary" mini @click.native="showBhrUnitPopup = true">></x-button>
        </x-input>
        <x-input title="保护人职位" v-model='bhrzw' text-align='right'></x-input>
        <x-input title="保护方式" v-model='bhfs' text-align='right'>
          <x-button slot="right" type="primary" mini @click.native="showBhfsPopup = true">></x-button>
        </x-input>
      </group>
    </group>
    <popup v-model="showBhrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='bhrPickerName' @on-change='onChangeBhrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showBhrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showBhrUnitPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='unitNameList' v-model='bhrUnitPickerName' @on-change='onChangeBhrUnitName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showBhrUnitPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showBhfsPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='fsList' v-model='bhfsPicker' @on-change='onChangeBhfs'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showBhfsPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showBdyyPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='bdList' v-model='bdyyPicker' @on-change='onChangeBdyy'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showBdyyPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
  </div>
</template>

<script>
import { Group, XInput, PopupPicker, Picker, Datetime, XTextarea, XAddress, ChinaAddressData, Cell, XSwitch, Checker, CheckerItem, Popup, XButton } from 'vux'
import { SET_RECORDBASE } from '../../mutationTypes'
export default {
  components: {
    Group,
    XInput,
    Picker,
    Datetime,
    XTextarea,
    XAddress,
    ChinaAddressData,
    Cell,
    XSwitch,
    PopupPicker,
    Popup,
    Checker,
    CheckerItem,
    XButton
  },
  computed: {
    unitList: function () {
      return this.$store.getters.GetterUnitState
    },
    userList: function () {
      return this.$store.getters.GetterUserState
    },
    unitNameList: function () {
      return this.$store.getters.GetterUnitNameState
    },
    userNameList: function () {
      return this.$store.getters.GetterUserNameState
    },
    showBdReason: function () {
      if (this.xczk[0] === '原始现场') {
        return false
      } else {
        return true
      }
    }
  },
  mounted: function () {
    if (this.$store.getters.GetterEntity.bh_flag === 1) {
      this.bhFlag = true
    }
    if (this.$store.getters.GetterEntity.light_info !== '') {
      this.lightList = this.$store.getters.GetterEntity.light_info.split('+')
    }
  },
  methods: {
    saveInfo () {
      let lightInfo = ''
      if (this.lightList.length !== 0) {
        this.lightList.forEach(function (item) {
          lightInfo += item + '+'
        })
        lightInfo = lightInfo.substring(0, lightInfo.length - 1)
      }
      let entity = {
        ky_unit: this.kyUnit.length === 0 ? 0 : parseInt(this.kyUnit[0]),
        jjr: this.jjr.length === 0 ? 0 : parseInt(this.jjr[0]),
        jjr_other: this.jjrOther,
        bg_unit: this.bjUnit.length === 0 ? 0 : parseInt(this.bjUnit[0]),
        ky_date: this.kyDate,
        bj_time: this.bjTime,
        kyks_time: this.kyksTime,
        kyjs_time: this.kyjsTime,
        xz: this.xz,
        fs_loc: this.fsqy,
        xc_loc: this.xcwz,
        xc_locpt: this.xcwzzb,
        weather_info: this.tq.length === 0 ? '' : this.tq[0],
        trend_info: this.fx.length === 0 ? '' : this.fx[0],
        temper_info: this.wd.length === 0 ? '' : this.wd[0],
        humidity_info: this.sd.length === 0 ? '' : this.sd[0],
        light_info: lightInfo,
        bh_flag: this.bhFlag ? 1 : 0,
        bhr: this.bhr.length === 0 ? 0 : parseInt(this.bhr[0]),
        bhr_name: this.bhrName,
        bhr_unit: this.bhrdw.length === 0 ? 0 : parseInt(this.bhrdw[0]),
        bhr_unit_name: this.bhrUnitName,
        bhr_pos: this.bhrzw,
        bh_function: this.bhfs,
        xc_info: this.xczk.length === 0 ? '' : this.xczk[0],
        bd_reason: this.bdyy.length === 0 ? '' : this.bdyy[0]
      }
      this.$store.commit(SET_RECORDBASE, entity)      
    },
    fetchXct () {
      Cordova.exec(this.onSuccessXct, this.onError, 'Interactive', 'shotMapInfo', [])
    },
    fetchPmt () {
      Cordova.exec(this.onSuccessPmt, this.onError, 'Interactive', 'shotMapInfo', [])
    },
    onSuccessXct: function (result) {
      var res = JSON.parse(result)
      
      var image = document.getElementById('xctImg')
      image.src = res.shot_picture_path
      image.width = 200
      image.height = 200
      
      var canvas = this.convertImageToCanvas(image)
      var blob = canvas.toDataURL('image/png')
      var fileName = this.$store.getters.GetterEntity.uuid + '_xct.png'

      this.uploadZT(blob, fileName)
      //alert(res.shot_picture_path)
    },
    onSuccessPmt: function (result) {
      var res = JSON.parse(result)
      
      var image = document.getElementById('pmtImg')
      image.src = res.shot_picture_path
      image.width = 200
      image.height = 200

      var canvas = this.convertImageToCanvas(image)
      var blob = canvas.toDataURL('image/png')
      var fileName = this.$store.getters.GetterEntity.uuid + '_pmt.png'

      this.uploadZT(blob, fileName)
      //alert(res.shot_picture_path)
    },
    onError: function (result) {
      this.$vux.toast.show({
        text: result,
        type: 'warn'
      })
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
    convertImageToCanvas(image){
      var canvas = document.createElement("canvas")
      canvas.width = image.width;
      canvas.height = image.height;
      // 坐标(0,0) 表示从此处开始绘制，相当于偏移。
      canvas.getContext("2d").drawImage(image, 0, 0)

      return canvas
    },
    uploadZT (blob, fileName) {
      //const canvas = document.querySelector('#canvas')
      //var src = canvas.toDataURL('image/png')
      //var blob = this.dataURItoBlob(src)
    
      //var req = new XMLHttpRequest()

      //req.open("POST", this.$store.getters.GetterBaseUrl + 'api/Files/singleUpload')

      //req.send(form)
      //var fileName = this.$store.getters.GetterEntity.uuid + '.png'
      var fd = new FormData();
      fd.append(fileName, blob);//fileData为自定义    
      
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
          alert("上传成功")
        } else {
          this.$vux.toast.show({
            text: res.data.message,
            type: 'warn'
          })          
        }
      }).catch(err => {
        this.$vux.toast.show({
          text: err,
          type: 'warn'
        })
      })
    },
    onChangeBhrName (value) {
      this.bhrName = value[0]
    },
    onChangeBhrUnitName (value) {
      this.bhrUnitName = value[0]
    },
    onChangeBhfs (value) {
      this.bhfs = value[0]
    },
    onChangeBdyy (value) {
      this.bdyy = value[0]
    },
    onUnitLoaded () {
    },
    onChange () {
    },
    onShow () {
    },
    onHide (type) {
    }
  },
  data () {
    return {
      showPopup: false,
      showBhrPopup: false,
      showBhrUnitPopup: false,
      showBhfsPopup: false,
      showBdyyPopup: false,
      bhrPickerName: [''],
      bhrUnitPickerName: [''],
      bhfsPicker: [''],
      bdyyPicker: [''],
      record_ky_no: this.$store.getters.GetterEntity.record_ky_no,
      record_jj_no: this.$store.getters.GetterEntity.record_jj_no,
      record_aj_no: this.$store.getters.GetterEntity.record_aj_no,
      xctSrc: this.$store.getters.GetterEntity.xct_src,
      pmtSrc: this.$store.getters.GetterEntity.pmt_src,
      kyUnit: this.$store.getters.GetterEntity.ky_unit === 0 ? [] : [this.$store.getters.GetterEntity.ky_unit + ''],
      bjTime: this.$store.getters.GetterEntity.bj_time,
      bjUnit: this.$store.getters.GetterEntity.bg_unit === 0 ? [] : [this.$store.getters.GetterEntity.bg_unit + ''],
      kyksTime: this.$store.getters.GetterEntity.kyks_time,
      kyjsTime: this.$store.getters.GetterEntity.kyjs_time,
      xz: this.$store.getters.GetterEntity.xz,
      kyDate: this.$store.getters.GetterEntity.ky_date,
      jjr: this.$store.getters.GetterEntity.jjr === 0 ? [] : [this.$store.getters.GetterEntity.jjr + ''],
      jjrOther: this.$store.getters.GetterEntity.jjr_other,
      fsqy: this.$store.getters.GetterEntity.fs_loc,
      tqList: [['晴', '阴', '多云', '雨']],
      fxList: [['无', '东', '南', '西', '北']],
      wdList: [['0 ℃', '1 ℃', '2 ℃', '3 ℃', '4 ℃', '5 ℃', '6 ℃', '7 ℃', '8 ℃', '9 ℃', '10 ℃', '11 ℃', '12 ℃', '13 ℃', '14 ℃', '15 ℃', '16 ℃', '17 ℃', '18 ℃', '19 ℃', '20 ℃', '21 ℃', '22 ℃', '23 ℃', '24 ℃', '25 ℃', '26 ℃', '27 ℃', '28 ℃', '29 ℃', '30 ℃', '31 ℃', '32 ℃', '33 ℃', '34 ℃', '35 ℃', '36 ℃', '37 ℃', '38 ℃', '39 ℃', '40 ℃']],
      sdList: [['10%', '20%', '30%', '40%', '50%', '60%', '70%', '80%', '90%', '100%']],
      lightList: [],
      addressData: ChinaAddressData,
      tq: this.$store.getters.GetterEntity.weather_info === '' ? [] : [this.$store.getters.GetterEntity.weather_info],
      fx: this.$store.getters.GetterEntity.trend_info === '' ? [] : [this.$store.getters.GetterEntity.trend_info],
      // gz: this.$store.getters.GetterEntity.light_info === '' ? [] : [this.$store.getters.GetterEntity.light_info],
      xcwz: this.$store.getters.GetterEntity.xc_loc,
      xcwzzb: this.$store.getters.GetterEntity.xc_locpt,
      wd: this.$store.getters.GetterEntity.temper_info === '' ? [] : [this.$store.getters.GetterEntity.temper_info],
      sd: this.$store.getters.GetterEntity.humidity_info === '' ? [] : [this.$store.getters.GetterEntity.humidity_info],
      bhFlag: false,
      fsList: [['专人看护现场，防止他人进入', '设立警戒带，划定禁行区域', '封锁出入口', '其他']],
      zkList: [['原始现场', '变动现场']],
      bdList: [['事主进入', '报案人进入', '公共区域', '补勘现场', '其他']],
      bhr: this.$store.getters.GetterEntity.bhr === 0 ? [] : [this.$store.getters.GetterEntity.bhr + ''],
      bhrName: this.$store.getters.GetterEntity.bhr_name,
      bhrUnitName: this.$store.getters.GetterEntity.bhr_unit_name,
      bhfs: this.$store.getters.GetterEntity.bh_function,
      bdyy: this.$store.getters.GetterEntity.bd_reason,
      bhrdw: this.$store.getters.GetterEntity.bhr_unit === 0 ? [] : [this.$store.getters.GetterEntity.bhr_unit + ''],
      xczk: this.$store.getters.GetterEntity.xc_info === '' ? [] : [this.$store.getters.GetterEntity.xc_info],
      bhrzw: this.$store.getters.GetterEntity.bhr_pos
    }
  }
}
</script>

<style>
  .contentinput{
    text-align: right
  }
  .demo4-item {
    background-color: #ddd;
    color: #222;
    font-size: 14px;
    padding: 5px 10px;
    margin-right: 10px;
    line-height: 18px;
    border-radius: 15px;
  }
  .demo4-item-selected {
    background-color: #FF3B3B;
    color: #fff;
  }
  .demo4-item-disabled {
    color: #999;
  }
</style>
