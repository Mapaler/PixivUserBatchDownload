# PixivUserBatchDownload
中文名：“[P站](//www.pixiv.net/member.php?id=3896348)画师个人作品批量下载工具”，简称 PUBD。

* #### 为什么要做 PUBD？
  PUBD 的理念是在阅览 Pixiv 的过程中，看见喜欢的画师，可以直接一键下载该画师的所有作品。

  融入网页的体验，不需要专门打开其他程序。
  
* #### PUBD 都有哪些功能？
  PUBD 的功能主要是可以下载画师所有公开作品，并且可以自定义下载路径、文件名。
  
  可以发送到家里的路由、租用的 VPS、家里的安卓智能电视等远程地址下载。（因为我单位上网收流量费😢）

![程序运行图](docs/images/preview.png)


## 程序下载
* [从 Greasy Fork 安装](//greasyfork.org/zh-CN/scripts/17879)(推荐)
* [从 GitHub 源文件安装](PixivUserBatchDownload.user.js)
> **用户样式CSS** 自 v5.8 开始已内置为自动加载，不再需要单独安装。

## 使用说明
请访问▶ **[PUBD Wiki](../../wiki)** ◀阅读。

### 程序结构与原理
PUBD 主体部分是采用 JavaScript 语言编写的用户脚本。

PUBD 通过 Pixiv 官方手机 APP 的后台 API 获取作者的作品列表，然后发送到指定的 Aria2 下载器下载，可选用第三方管理器对 Aria2 状态进行管理。

下载流程的结构如下图  
![结构图](docs/images/structure.jpg)

## 后续功能开发状态
因为以下等原因，本项目已进入事实停止维护状态，所以后续的开发展望这几年没有添加任何代码。
1. P站被封锁
2. 色图太多，下了也看不过来
3. 本职工作
4. 有孩子了
5. [肝手游](https://github.com/Mapaler/PADDashFormation/)
6. 工作太忙了，手游都不玩了。

<details>
<summary>PUBD 5 已实现</summary>

- [x] 下载逐项发送（不卡死）
- [x] 输出文本信息（下载列表）
- [x] 子菜单快速完成操作（已开发但暂未使用）
- [x] 仅下载当前一幅作品
- [x] 下载过滤器
- [x] 点击通知自动关闭页面
- [x] 扩展菜单内直接打开程序
- [x] 使用 system.multicall 加快 Aria2 请求速度
- [x] 适应新的 oAuth 2.0 登录模式

</details>

<details>
<summary>PUBD 6 计划</summary>

- [x] 自动标记已快速收藏作者的推荐作品(目前只是添加明式标记，将来把多作者下载做了后，改成切换显示隐藏)
- [x] **使用 ES6 原生模板字符串替代 PUBD 自己实现的掩码写法**（已开发暂未启用，准备 PUBD 6 迁移）
- [ ] **废除自定义掩码，并修改为一个可以任意书写的代码块，可自由定义数组和自定义函数。**
- [ ] **多画师批量下载**(储存画师>储存解析到的数据>一起发送到 Aria2) (IndexedDB 太复杂看不懂，但是是目前努力的方向，用 GM_setValue 储存可能数据多了会太卡。)
- [ ] 每位画师的额外下载内容（用于下载画师头像、背景头图等）；每个动图的额外下载内容（用于输出帧率）

</details>

<details>
<summary>未列上开发日程的计划</summary>

- [ ] 自动清除 Aria2 下载完成项目 (暂时不是很必要)
- [ ] WebSocket 协议 (暂时不是很必要)
- [ ] 多语种支持（老旧代码太多，暂时无法搞）

</details>

## License|许可协议
PixivUserBatchDownload © Mapaler 2021

> 此程序是免费软件。你可以将它根据“GNU通用公共许可证第三版(GPLv3)”重新分发和/或修改。

* Aria2 操作对象代码来自 [ThunderLixianExporter](//github.com/binux/ThunderLixianExporter)。
* Pixiv APP-API 分析灵感来自 [PixivPy](//github.com/upbit/pixivpy)。

## 友情链接
**[![](https://raw.githubusercontent.com/xuejianxianzun/PixivBatchDownloader/master/static/icon/logo48.png)PixivBatchDownloader](//github.com/xuejianxianzun/PixivBatchDownloader)**  
功能介绍：  
* 支持多种页面里的批量下载，可以设置多种筛选条件
* 有一些辅助功能，如去除广告、快速收藏、看图模式等
* 下载不依赖第三方工具
* 支持多语言