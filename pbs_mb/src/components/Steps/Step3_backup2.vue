<template>
  <div>
    <div v-if="~this.isAdd" v-for='oriFile in oriFiles'>
      <x-img v-bind:src="onGetFile(oriFile.file_url)" class="preview" :offset="1000"></x-img>
      <cell v-bind:title="onRenderFileType(oriFile.file_type)" v-bind:value="oriFile.file_hint"></cell>
    </div>
    <group>
      <cell title="本次已上传图像:  ">{{alreadyPicNum}}  张</cell>
    </group>
    <x-hr></x-hr>
    <x-button type="primary" @click.native="chooseFile(1)" >选择现场方位</x-button>
    <vue-file-upload v-bind:url="renderUrl()" ref="vueFileUploader1" v-bind:filters = "filters" label ='选择现场方位'
    v-bind:events = 'cbEvents'
    v-bind:request-options = "reqopts" v-on:onAdd = "onAddFile1"  class='uploadbutton' ></vue-file-upload>
    <div v-for='(file1,index) in files1'>
      <group>
        <x-img v-bind:src="onPreView(file1)" class="preview"></x-img>
        <x-button type="warn" mini @click.native="onCancelFile(file1,index,$event)" >取消该图片</x-button>
        <x-input title="图片说明:" :debounce="500" placeholder="请填写说明" @on-change="onChangePicHint($event,file1)" text-align='right' class="input-hint"></x-input>
        <br>
        <checker
          default-item-class="demo5-item" @on-change="onChangePicType($event,file1)"
          selected-item-class="demo5-item-selected" value="2001" >
            <checker-item value="2001">现场方位</checker-item>
            <checker-item value="2002" disabled >现场概貌</checker-item>
            <checker-item value="2003" disabled >重点部位</checker-item>
        </checker>
        <br>
        <x-progress v-bind:percent='file1.progress' :show-cancel="false"></x-progress>
        <cell title="" v-bind:value="onStatus(file1)"></cell>
      </group>
    </div>
    <x-hr></x-hr>
    <vue-file-upload v-bind:url="renderUrl()" ref="vueFileUploader2" v-bind:filters = "filters" label ='选择现场概貌'
    v-bind:events = 'cbEvents' v-bind:request-options = "reqopts" v-on:onAdd = "onAddFile2" class='uploadbutton' ></vue-file-upload>
    <div v-for='(file2,index) in files2'>
      <group>
        <x-img v-bind:src="onPreView(file2)" class="preview"></x-img>
        <x-button type="warn" mini @click.native="onCancelFile(file2,index,$event)" >取消该图片</x-button>
        <x-input title="图片说明:" :debounce="500" placeholder="请填写说明" @on-change="onChangePicHint($event,file2)" text-align='right' class="input-hint"></x-input>
        <br>
        <checker
          default-item-class="demo5-item" @on-change="onChangePicType($event,file2)"
          selected-item-class="demo5-item-selected" value="2002" >
            <checker-item value="2001" disabled>现场方位</checker-item>
            <checker-item value="2002"  >现场概貌</checker-item>
            <checker-item value="2003" disabled >重点部位</checker-item>
        </checker>
        <br>
        <x-progress v-bind:percent='file2.progress' :show-cancel="false"></x-progress>
        <cell title="" v-bind:value="onStatus(file2)"></cell>
      </group>
    </div>
    <x-hr></x-hr>
    <vue-file-upload v-bind:url="renderUrl()" ref="vueFileUploader3" v-bind:filters = "filters" label ='选择重点部位'
    v-bind:events = 'cbEvents' v-bind:request-options = "reqopts" v-on:onAdd = "onAddFile3" class='uploadbutton' ></vue-file-upload>
    <div v-for='(file3,index) in files3'>
      <group>
        <x-img v-bind:src="onPreView(file3)" class="preview"></x-img>
        <x-button type="warn" mini @click.native="onCancelFile(file3,index,$event)" >取消该图片</x-button>
        <x-input title="图片说明:" :debounce="500" placeholder="请填写说明" @on-change="onChangePicHint($event,file3)" text-align='right' class="input-hint"></x-input>
        <br>
        <checker
          default-item-class="demo5-item" @on-change="onChangePicType($event,file3)"
          selected-item-class="demo5-item-selected" value="2003" >
            <checker-item value="2001" disabled>现场方位</checker-item>
            <checker-item value="2002" disabled >现场概貌</checker-item>
            <checker-item value="2003"  >重点部位</checker-item>
        </checker>
        <br>
        <x-progress v-bind:percent='file3.progress' :show-cancel="false"></x-progress>
        <cell title="" v-bind:value="onStatus(file3)"></cell>
      </group>
    </div>
  </div>
</template>

<script>
import { Group, XInput, XSwitch, PopupPicker, XHr, XButton, Flexbox, FlexboxItem, XProgress, Cell, XImg, Checker, CheckerItem } from 'vux'
import VueFileUpload from 'vue-file-upload'

export default {
  components: {
    Group,
    XInput,
    PopupPicker,
    VueFileUpload,
    XHr,
    XButton,
    Flexbox,
    FlexboxItem,
    XProgress,
    XImg,
    Cell,
    Checker,
    CheckerItem,
    XSwitch
  },
  computed: {
    alreadyPicNum: function () {
      var num = 0
      this.completeNames.forEach(function (item) {
        if (item.file_url !== '') {
          num++
        }
      })
      return num
    }
  },
  mounted: function () {
    this.isAdd = this.$route.query.isAdd
    if (~(this.isAdd)) {
      this.$vux.loading.show({
        text: '读取文件列表'
      })
      this.getOriFiles()
    }
    let tmp = []
    if (this.$store.getters.GetterFileList.length !== 0) {
      this.$store.getters.GetterFileList.forEach(function (item) {
        let tt = item
        tmp.push(tt)
      })
    }
    this.completeNames = tmp
    this.files1 = []
    this.files2 = []
    this.files3 = []
  },
  methods: {    
    renderUrl () {
      return this.$store.getters.GetterBaseUrl + 'api/Files/singleUpload'
    },
    onStatus (file) {
      if (file.isSuccess) {
        return '上传成功'
      } else if (file.isError) {
        return '上传失败'
      } else if (file.isUploading) {
        return '正在上传'
      } else {
        return '待上传'
      }
    },
    getOriFiles () {
      let uuid = this.$store.getters.GetterEntity.uuid
      this.$http({
        url: this.$store.getters.GetterBaseUrl + 'api/Files/GetFileList',
        method: 'GET',
        emulateJSON: true,
        params: {
          uuid: uuid
        },
        headers: {
          contentType: 'application/x-www-form-urlencoded',
          token: this.$store.getters.GetterToken
        }
      }).then(function (res) {
        if (res.data.code === 100) {
          this.oriFiles = res.data.resultList
          this.$vux.loading.hide()
        } else {
          this.$vux.loading.hide()
          this.$vux.toast.show({
            text: res.data.message,
            type: 'warn'
          })
        }
      }).catch(err => {
        this.$vux.loading.hide()
        this.$vux.toast.show({
          text: err,
          type: 'warn'
        })
      })
    },
    onGetFile (fileUrl) {
      let picUrl = this.$store.getters.GetterBaseUrl + 'api/GetFile?fileName=' + fileUrl + '&uuid=' + this.$store.getters.GetterEntity.uuid
      return picUrl
    },
    onRenderFileType (type) {
      switch (type) {
        case 2001:
          return '现场方位'
        case 2002:
          return '现场概貌'
        case 2003:
          return '重点部位'
        case 2004:
          return '现场制图'
        default:
          return '其他'
      }
    },
    onBtnStatus (file) {
      if (file.isSuccess) {
        return true
      } else if (file.isUploading) {
        return true
      } else {
        return false
      }
    },
    onPreView (file) {
      let src = window.URL.createObjectURL(file.file)
      return src
    },
    onCancelFile (file, index, event) {
      event.target.parentElement.parentElement.style.display = 'none'
      let cplnIndex = 0
      for (let i = 0; i < this.completeNames.length; i++) {
        if (this.completeNames[i] === file.file.name) {
          cplnIndex = i
          break
        }
      }
      this.completeNames.splice(cplnIndex, 1)
      file.remove()
    },
    onAddFile1 (files) {
      this.completeNames.forEach(function (item) {
        if (item.file_name === files[files.length - 1].file.name) {
          item.file_type = 2001
        }
      })
      this.files1.push(files[files.length - 1])
    },
    onAddFile2 (files) {
      this.completeNames.forEach(function (item) {
        if (item.file_name === files[files.length - 1].file.name) {
          item.file_type = 2002
        }
      })
      this.files2.push(files[files.length - 1])
    },
    onAddFile3 (files) {
      this.completeNames.forEach(function (item) {
        if (item.file_name === files[files.length - 1].file.name) {
          item.file_type = 2003
        }
      })
      this.files3.push(files[files.length - 1])
    },
    uploadItem (file) {
      // 单个文件上传
      file.upload()
    },
    deleteItem (file) {
      file.remove()
    },
    onChangePicHint (val, file) {
      this.completeNames.forEach(function (item) {
        if (item.file_name === file.file.name) {
          item.file_hint = val
        }
      })
    },
    onChangePicType (val, file) {
      this.completeNames.forEach(function (item) {
        if (item.file_name === file.file.name) {
          item.file_type = val[0]
        }
      })
    },
    updateResponseInfo (file, fileUrl) {
      /*
      this.completeNames.forEach(function (item) {
        if (item.file_name === file.file.name) {
          item.file_url = fileUrl
        }
      })
      */
      this.completeNames.every(function (item) {
        if (item.file_name === file.file.name && item.file_url === '') {
          item.file_url = fileUrl
          return false
        } else {
          return true
        }
      })
    },
    onUploadFiles () {
      // 上传所有文件
      this.$refs.vueFileUploader1.uploadAll()
      // this.$refs.vueFileUploader2.uploadAll()
      // this.$refs.vueFileUploader3.uploadAll()
    }
  },
  data () {
    return {
      testName: 'TTT',
      isAdd: true,
      btnstate: false,
      completeNames: [],
      oriFiles: [],
      files1: [],
      files2: [],
      files3: [],
      typeList: [[{
        value: '2001',
        name: '现场方位'
      },
      {
        value: '2002',
        name: '现场概貌'
      },
      {
        value: '2003',
        name: '重点部位'
      },
      {
        value: '2004',
        name: '现场制图'
      },
      {
        value: '2005',
        name: '其他'
      }]],
      // 文件过滤器，只能上传图片
      filters: [
        {
          name: 'imageFilter',
          fn (file) {
            var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|'
            return '|jpg|png|jpeg|bmp|gif|tif|'.indexOf(type) !== -1
          }
        }
      ],
      cbEvents: {
        onCompleteUpload: (file, response, status, header) => {
          if (status === 200) {
            this.updateResponseInfo(file, response.file)
          }
        },
        onAddFileSuccess: (file) => {
          let fileName = file.file.name
          this.completeNames.push({
            file_name: fileName,
            file_url: '',
            file_hint: '',
            file_type: 0
          })
          this.testName += file.file.name
          console.log(file.file)
          //this.onUploadFiles()
        }
      },
      // xhr请求附带参数
      reqopts: {        
        responseType: 'json',
        withCredentials: false
      }
    }
  }
}
</script>

<style>
  .uploadbutton{
  }
  .preview {
    width: 300px;
    height: 300px
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
    background: #ffffff url(../../assets/active.png) no-repeat right bottom;
    border-color: #ff4a00;
  }
  .input-hint {
    border-radius: 3px;
    border: 1px solid #ccc;
    background-color: #fff;
  }
</style>
