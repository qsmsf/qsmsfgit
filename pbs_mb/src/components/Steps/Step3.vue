<template>
  <div>
    <div v-if="~this.isAdd" v-for='oriFile in oriFiles'>
      <x-img v-bind:src="onGetFile(oriFile.file_url)" class="preview" :offset="1000"></x-img>
      <cell v-bind:title="onRenderFileType(oriFile.file_type)" v-bind:value="oriFile.file_hint"></cell>
    </div>
    <img class="preview" src="" id="test"/>
    <x-hr></x-hr>
    <x-button type="primary" @click.native="showSelection = true" >增加照片</x-button>
    <div v-for='(file, index) in uploaderFiles'>
      <group>
        <img class="preview" :src="file.file_path"/>
        <x-img v-bind:src="file.file_path" class="preview"></x-img>
        <x-button type="warn" mini @click.native="onCancelFile(index,$event)" >取消该图片</x-button>
        <x-input title="图片说明:" :debounce="500" placeholder="请填写说明" @on-change="onChangePicHint($event,index)" text-align='right' v-bind:value="file.file_hint" class="input-hint"></x-input>
        <br>
        <checker
          default-item-class="demo5-item" @on-change="onChangePicType($event,index)"
          selected-item-class="demo5-item-selected" v-bind:value="file.file_type" >
            <checker-item value="2001">现场方位</checker-item>
            <checker-item value="2002" >现场概貌</checker-item>
            <checker-item value="2003" >重点部位</checker-item>
        </checker>
      </group>
    </div>
    <actionsheet v-model="showSelection" :menus="menusForChooseFile" @on-click-menu="getFile" show-cancel></actionsheet>
    
  </div>
</template>

<script>
import { Group, XInput, XSwitch, PopupPicker, XHr, XButton, Flexbox, FlexboxItem, XProgress, Cell, XImg, Checker, CheckerItem, Actionsheet } from 'vux'

export default {
  components: {
    Group,
    XInput,
    PopupPicker,    
    XHr,
    XButton,
    Flexbox,
    FlexboxItem,
    XProgress,
    XImg,
    Cell,
    Checker,
    CheckerItem,
    XSwitch,
    Actionsheet
  },
  computed: {    
  },
  mounted: function () {
    this.isAdd = this.$route.query.isAdd
    if ((this.isAdd)) {
      this.$vux.loading.show({
        text: '读取文件列表'
      })
      this.getOriFiles()
    }
    this.uploaderFiles = this.$store.getters.GetterFileList
    var saveDirectory = "PBSFiles"
    this.getSavePath(saveDirectory)
  },
  methods: {
//     getFile()
//     {
//         navigator.camera.getPicture(function(imageURI) {
//                     window.resolveLocalFileSystemURI(imageURI, function(fileEntry) {
//                         fileEntry.file(function(fileObj) {
//                             uploadTest(fileObj);
//                         });
//                     });
//                 },
//                 function(message) {
// //                    alert('get picture failed');
//                 },
//                 {
//                     quality: 50,
//                     targetWidth: 400,
//                     targetHeight: 500,
//                     destinationType: navigator.camera.DestinationType.FILE_URI,
//                     sourceType: navigator.camera.PictureSourceType.PHOTOLIBRARY
//                 }
//         );
//     },
    getSavePath (saveDirectory) {
        app.getAppDirectoryEntry(function(res){
        //区分平台，并将相应的目录保存到全局,方便下面下载的时候使用
    //            alert(JSON.stringify(res));     //wangxi
            if(window.devicePlatform=="android"){
                this.savePath = res.sdcard + "/" + saveDirectory;
            }
        });
    },
    getFile (type) {
      console.log(type)
      if(type === 'fromPhotos'){

        // var params = {"action" : "choice_file_activity"}
        // //var params2 = {"action" : "xinghuo_back_camera_activity"}

        // app.szgaplugin.startActivityForResult(params, this.addFile, function (err) {
        //     //alert(JSON.stringify(err));
        // })
        navigator.camera.getPicture(addFile, getFileError, {
            quality: 100,
            //targetWidth: 600,
            //targetHeight: 1000,
            destinationType: navigator.camera.DestinationType.FILE_URI,
            sourceType: 0,
            encodingType: navigator.camera.EncodingType.JPEG
        })
      }else if(type === 'takePhotos') {
        navigator.camera.getPicture(addFile, getFileError, {
            quality: 100,
            //targetWidth: 600,
            //targetHeight: 1000,
            destinationType: navigator.camera.DestinationType.FILE_URI,
            sourceType: navigator.camera.PictureSourceType.CAMERA,
            encodingType: navigator.camera.EncodingType.JPEG
        })    
      }
    },
    addFile (res) {
      //var ret = eval("(" + res + ")")
        //getPlateInfo(ret)
        var arr = res.split('/')
        var file = {
          file_name : arr[arr.length-1],
          file_path : res,
          file_url: '',
          file_hint : 'fsdfsdf',
          file_type : '2001'
        }
      //测试图像显示
      var image = document.getElementById('test')
      image.src = res
      alert(ret.file_path)

      this.uploaderFiles.push(file)
      var options = new FileUploadOptions();
      options.fileKey = file.file_name;     //用于设置参数，服务端的Request字串
      options.fileName = file.file_name;    //希望文件存储到服务器所用的文件名
      options.mimeType = "image/jpeg";  //图片格式

      var uri = encodeURI(_url + '/api/Files/MutilUpload')
      this.$vux.loading.show({
        text: '文件上传中'
      })
      xh.uploadFile(fileEntry.fullPath,uri,uploadOK,onFail,options)        
        
    },
    getFileError (err) {
        //alert(JSON.stringify(err));
    },
    uploadOK (msg) {
        alert(JSON.stringify(msg))
        app.hint("上传图片成功!")
        var data = eval("(" + msg.response + ")")

        // var Pfname = data.obj[0].pfname
        // imgName = Pfname.substr(Pfname.lastIndexOf('/') + 1)
        // this.uploaderFiles[this.uploaderFiles.length-1].file_url = imgName

        //mediaFiles.substr(mediaFiles.lastIndexOf('/') + 1)
        this.$vux.loading.hide()
    },
    onFail(err) {
        app.hint("上传图片失败!");
        alert(JSON.stringify(err));
        this.$vux.loading.hide()
    },
    getOriFiles () {
      let uuid = this.$store.getters.GetterEntity.uuid
      if(isProxy){
          let headers = {
            token: this.$store.getters.GetterToken
          }
          let headerdata = JSON.stringify(headers)
          
          xh.get(_url + '/api/Files/GetFileList',
            {
              uuid: uuid,
              headerParameter: headerdata
            },
            function(res){
              let ret = eval("(" + res + ")")
              if (ret.returnValue.code === 100) {
                this.oriFiles = ret.returnValue.resultList
                this.$vux.loading.hide()
              } else {
                this.$vux.loading.hide()
                this.$vux.toast.show({
                  text: ret.returnValue.message,
                  type: 'warn'
                })
              }
            }, 
            function (err) {
              this.$vux.loading.hide()
              this.$vux.toast.show({
                text: err,
                type: 'warn'
              })
            })
        }else{
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
        }
    },
    uploadTest(filePath){
        var options = new FileUploadOptions();
        options.fileKey = "uploadFile";     //用于设置参数，服务端的Request字串
        options.fileName = "testPicture2.jpg";    //希望文件存储到服务器所用的文件名
        options.mimeType = "image/jpeg";  //图片格式
        // + "?APP_URL=" + "http://10.42.0.235:9001/myProject/myServlet" + "?"
      //  var uri = encodeURI("http://10.42.0.235:9001/myProject/myServlet"); //接收上传图片的地址
        var uri = encodeURI(_url + '/api/Files/MutilUpload')
            options.chunkedMode = false;  //如果上传的文件大小未知时，必须带上此参数，否则后台无法获取文件流
//            var uri = encodeURI("http://172.28.0.56:9001/myProject/myServlet" + "?loginId=123&APP_URL=" + _url + "/testApp/uploadTest");  //接收上传图片的地址

        var params = new Object();    //通过HTTP请求发送到服务器的一系列可选键/值对。（对象类型）
//        params.APP_URL = "http://10.42.0.235:9001/myProject/myServlet?loginId=123&id=222";    //自定义参数
        options.params = params;

        app.hint("上传 : " + fileEntry.fullPath);
//            app.hint("上传" + uri);
        
        xh.uploadFile(fileEntry.fullPath,uri,uploadOK,onFail,options);
        function uploadOK(msg){
            alert(JSON.stringify(msg));
            app.hint("上传图片成功!");
            var data = eval("(" + msg.response + ")");
            var Pfname = data.obj[0].pfname
            imgName = Pfname.substr(Pfname.lastIndexOf('/') + 1);
            //mediaFiles.substr(mediaFiles.lastIndexOf('/') + 1)
        }
        function onFail(err) {
            app.hint("上传图片失败!");
            alert(JSON.stringify(err));
        }

    },
    onGetFile (fileUrl) {
      let picUrl = this.$store.getters.GetterBaseUrl + 'api/GetFile?fileName=' + fileUrl + '&uuid=' + this.$store.getters.GetterEntity.uuid
      //xh.get()
      if(isProxy){

      }else{
        
        return picUrl
      }      
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
    onChangePicHint (val, index) {
      this.uploaderFiles[index].file_hint = val        
    },
    onChangePicType (val, index) {
      this.uploaderFiles[index].file_type = val
    },
  },
  data () {
    return {
      isAdd: true,
      uploaderFiles: [],      
      oriFiles: [],     
      showSelection: false,
      menusForChooseFile: {
        fromPhotos: '从相册选择',
        takePhotos: '拍照'
      },
      savePath: '',
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
      }]]      
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
