# Docker简介

- 参考：https://www.cnblogs.com/chiangchou/p/docker.html
- 博主：https://juejin.cn/user/2612095358879895/posts

## 1、什么是DOCKER

Docker 是使用最广泛的开源容器引擎，它彻底释放了计算虚拟化的威力，极大提高了应用的运行效率，降低了云计算资源供应的成本！ 使用 Docker，可以让应用的部署、测试和分发都变得前所未有的高效和轻松！

Docker 使用 Google 公司推出的 Go 语言 进行开发实现，基于 Linux 内核的 cgroup，namespace，以及 AUFS 类的 Union FS 等技术，**对进程进行封装隔离，属于操作系统层面的虚拟化技术。由于隔离的进程独立于宿主和其它的隔离的进程，因此也称其为容器。**

Docker 在容器的基础上，进行了进一步的封装，从文件系统、网络互联到进程隔离等等，极大的简化了容器的创建和维护。使得 Docker 技术比虚拟机技术更为轻便、快捷。

## 2、为什么要用 Docker

① 更高效的利用系统资源：由于容器不需要进行硬件虚拟以及运行完整操作系统等额外开销，Docker 对系统资源的利用率更高。

② 更快速的启动时间：Docker 容器应用，由于直接运行于宿主内核，无需启动完整的操作系统，因此可以做到秒级、甚至毫秒级的启动时间。

③ 一致的运行环境：Docker 的镜像提供了除内核外完整的运行时环境，确保了应用运行环境一致性。

④ 持续交付和部署：使用 Docker 可以通过定制应用镜像来实现持续集成、持续交付、部署。一次创建或配置，可以在任意地方正常运行。

⑤ 更轻松的迁移：Docker 确保了执行环境的一致性，使得应用的迁移更加容易。Docker 可以在很多平台上运行，无论是物理机、虚拟机、公有云、私有云，甚至是笔记本，其运行结果是一致的。

## 3、Docker 基本组成

① 镜像(Images)

Docker 镜像是一个特殊的文件系统，除了提供容器运行时所需的程序、库、资源、配置等文件外，还包含了一些为运行时准备的一些配置参数（如匿名卷、环境变量、用户等）。镜像不包含任何动态数据，其内容在构建之后也不会被改变。

Docker 设计时，充分利用 Union FS 的技术，将其设计为分层存储的架构，Docker 镜像由多层文件系统联合组成。镜像构建时，会一层层构建，前一层是后一层的基础。每一层构建完就不会再发生改变，后一层上的任何改变只发生在自己这一层。

② 容器(Container)

镜像（Image）和容器（Container）的关系，就像是面向对象程序设计中的 类 和 实例 一样，镜像是静态的定义，容器是镜像运行时的实体。容器可以被创建、启动、停止、删除、暂停等。

容器的实质是进程，但与直接在宿主执行的进程不同，容器进程运行于属于自己的独立的 命名空间。因此容器可以拥有自己的 root 文件系统、自己的网络配置、自己的进程空间，甚至自己的用户 ID 空间。容器内的进程是运行在一个隔离的环境里，使用起来，就好像是在一个独立于宿主的系统下操作一样。

每一个容器运行时，是以镜像为基础层，在其上创建一个当前容器的存储层，我们可以称这个为容器运行时读写而准备的存储层为容器存储层。容器存储层的生存周期和容器一样，容器消亡时，容器存储层也随之消亡。因此，任何保存于容器存储层的信息都会随容器删除而丢失。
按照 Docker 最佳实践的要求，容器不应该向其存储层内写入任何数据，容器存储层要保持无状态化。所有的文件写入操作，都应该使用 数据卷（Volume）、或者绑定宿主目录，在这些位置的读写会跳过容器存储层，直接对宿主(或网络存储)发生读写，其性能和稳定性更高。
数据卷的生存周期独立于容器，容器消亡，数据卷不会消亡。因此，使用数据卷后，容器可以随意删除、重新 run ，数据却不会丢失。

③ 镜像仓库(Registry)

镜像仓库是一个集中的存储、分发镜像的服务。一个 Docker Registry 中可以包含多个仓库（Repository）；每个仓库可以包含多个标签（Tag）；每个标签对应一个镜像。
通常，一个仓库会包含同一个软件不同版本的镜像，而标签就常用于对应该软件的各个版本。我们可以通过 <仓库名>:<标签> 的格式来指定具体是这个软件哪个版本的镜像。如果不给出标签，将以 latest 作为默认标签。

最常使用的 Registry 公开服务是官方的 Docker Hub，这也是默认的 Registry，并拥有大量的高质量的官方镜像。用户还可以在本地搭建私有 Docker Registry。Docker 官方提供了 Docker Registry 镜像，可以直接使用做为私有 Registry 服务。

① 镜像(Images)

Docker 镜像是一个特殊的文件系统，除了提供容器运行时所需的程序、库、资源、配置等文件外，还包含了一些为运行时准备的一些配置参数（如匿名卷、环境变量、用户等）。镜像不包含任何动态数据，其内容在构建之后也不会被改变。

Docker 设计时，充分利用 Union FS 的技术，将其设计为分层存储的架构，Docker 镜像由多层文件系统联合组成。镜像构建时，会一层层构建，前一层是后一层的基础。每一层构建完就不会再发生改变，后一层上的任何改变只发生在自己这一层。

② 容器(Container)

镜像（Image）和容器（Container）的关系，就像是面向对象程序设计中的 类 和 实例 一样，镜像是静态的定义，容器是镜像运行时的实体。容器可以被创建、启动、停止、删除、暂停等。

容器的实质是进程，但与直接在宿主执行的进程不同，容器进程运行于属于自己的独立的 命名空间。因此容器可以拥有自己的 root 文件系统、自己的网络配置、自己的进程空间，甚至自己的用户 ID 空间。容器内的进程是运行在一个隔离的环境里，使用起来，就好像是在一个独立于宿主的系统下操作一样。

每一个容器运行时，是以镜像为基础层，在其上创建一个当前容器的存储层，我们可以称这个为容器运行时读写而准备的存储层为容器存储层。容器存储层的生存周期和容器一样，容器消亡时，容器存储层也随之消亡。因此，任何保存于容器存储层的信息都会随容器删除而丢失。
按照 Docker 最佳实践的要求，容器不应该向其存储层内写入任何数据，容器存储层要保持无状态化。所有的文件写入操作，都应该使用 数据卷（Volume）、或者绑定宿主目录，在这些位置的读写会跳过容器存储层，直接对宿主(或网络存储)发生读写，其性能和稳定性更高。
数据卷的生存周期独立于容器，容器消亡，数据卷不会消亡。因此，使用数据卷后，容器可以随意删除、重新 run ，数据却不会丢失。

③ 镜像仓库(Registry)

镜像仓库是一个集中的存储、分发镜像的服务。一个 Docker Registry 中可以包含多个仓库（Repository）；每个仓库可以包含多个标签（Tag）；每个标签对应一个镜像。
通常，一个仓库会包含同一个软件不同版本的镜像，而标签就常用于对应该软件的各个版本。我们可以通过 <仓库名>:<标签> 的格式来指定具体是这个软件哪个版本的镜像。如果不给出标签，将以 latest 作为默认标签。

最常使用的 Registry 公开服务是官方的 Docker Hub，这也是默认的 Registry，并拥有大量的高质量的官方镜像。用户还可以在本地搭建私有 Docker Registry。Docker 官方提供了 Docker Registry 镜像，可以直接使用做为私有 Registry 服务。

# Docker 安装

Docker 版本包含 社区版和企业版，我们日常使用社区版足够。Docker 社区版各个环境的安装参考官方文档：https://docs.docker.com/install/。后面的学习在 Linux CentOS7 上测试

-  服务器和工具：我这里连接的是阿里云服务器ECS，远程名命令用的是微软应用商店的Termius插件
  ![](F:\MyGithub\Docker\Img\1.png)

## CentOS7 安装步骤

① 卸载旧版本

```ce
yum remove docker \
                  docker-client \
                  docker-client-latest \
                  docker-common \
                  docker-latest \
                  docker-latest-logrotate \
                  docker-logrotate \
                  docker-engine
```

② 安装依赖包

```
yum install -y yum-utils
```

③ 安装 Docker 软件包源（设置镜像的仓库）

```shell
um-config-manager \
    --add-repo \
    https://download.docker.com/linux/centos/docker-ce.repo  --默认是国外的

#如果没有vpn 建议安装阿里云的    
yum-config-manager \
 --add-repo \
 http://mirrors.aliyun.com/docker-ce/linux/centos/docker-ce.repo
#更新yum 索引安装包
 yum makecache fast 
```

④ 安装 Docker CE

```shell
yum install docker-ce docker-ce-cli containerd.io
```

⑤ 启动 Docker 服务

```shell
systemctl start docker
```

⑥ 设置开机启动

```shell
systemctl enable docker
```

⑦ 验证安装是否成功

```shell
![3](F:\MyGithub\Docker\Img\3.png)docker -v
docker info
```

![](F:\MyGithub\Docker\Img\2.png)

# Docker 命令

- 参考同目录的docker的word文档命令

通过 **--help** 参数可以看到 docker 提供了哪些命令，可以看到 docker 的用法是 **docker [选项] 命令 。**

命令有两种形式，Management Commands 是子命令形式，每个命令下还有子命令；Commands 是直接命令，相当于子命令的简化形式。

![](F:\MyGithub\Docker\Img\4.png)

继续查看 Management Commands 有哪些子命令，例如查看 image 的子命令。docker image ls 等同于 docker images，docker image pull 等同于 docker pull。

![](F:\MyGithub\Docker\Img\5.png)

# 镜像

## 镜像简介

镜像包含了一个软件的运行环境，是一个不包含Linux内核而又精简的Linux操作系统，一个镜像可以创建N个容器。

镜像是一个分层存储的架构，由多层文件系统联合组成。镜像构建时，会一层层构建，前一层是后一层的基础。

从下载过程中可以看到，镜像是由多层存储所构成。下载也是一层层的去下载，并非单一文件。下载过程中给出了每一层的 ID 的前 12 位。并且下载结束后，给出该镜像完整的 sha256 的摘要，以确保下载一致性

![](F:\MyGithub\Docker\Img\6.png)

通过 **docker history <ID/NAME>** 查看镜像中各层内容及大小，每层会对应着 Dockerfile 中的一条指令。

![](F:\MyGithub\Docker\Img\7.png)

由于 Docker 镜像是多层存储结构，并且可以继承、复用，因此不同镜像可能会因为使用相同的基础镜像，从而拥有共同的层。由于 Docker 使用 Union FS，相同的层只需要保存一份即可，因此实际镜像硬盘占用空间很可能要比这个列表镜像大小的总和要小的多。

![](F:\MyGithub\Docker\Img\8.png)

## 镜像管理

Docker 运行容器前需要本地存在对应的镜像，如果镜像不存在本地，Docker 会从镜像仓库下载，默认是 Docker Hub 公共注册服务器中的仓库。

Docker Hub 是由 Docker 公司负责维护的公共注册中心，包含大量的优质容器镜像，Docker 工具默认从这个公共镜像库下载镜像。下载的镜像如何使用可以参考官方文档。地址：https://hub.docker.com/explore

如果从 Docker Hub 下载镜像非常缓慢，可以先配置镜像加速器，参考：https://www.daocloud.io/mirror

Linux下通过以下命令配置镜像站：

```shell
# curl -sSL https://get.daocloud.io/daotools/set_mirror.sh | sh -s http://f1361db2.m.daocloud.io

# systemctl restart docker
```

### ① 搜索镜像 

**docker search <NAME> [选项]**

OFFICIAL：是否官方版本；

![](F:\MyGithub\Docker\Img\9.png)

###  ② 下载镜像 

**docker pull [选项] [Docker Registry地址]<仓库名>:<标签>**

Docker Registry地址：地址的格式一般是 <域名/IP>[:端口号] 。默认地址是Docker Hub。

仓库名：仓库名是两段式名称，既 <用户名>/<软件名> 。对于 Docker Hub，如果不给出用户名，则默认为 library ，也就是官方镜像。

![](F:\MyGithub\Docker\Img\10.png)

### ③ 列出本地镜像 

**docker images [选项]**

TAG：标签版本；IMAGE ID：镜像ID；SIZE：镜像大小；

![](F:\MyGithub\Docker\Img\11.png)

- 查看虚悬镜像（镜像既没有仓库名，也没有标签，显示为 <none>）：**docker images -f dangling=true**
- 默认的 docker images 列表中只会显示顶层镜像，如果希望显示包括中间层镜像在内的所有镜像：**docker images -a**
- 只列出镜像ID：**docker images -q**
- 列出部分镜像：**docker images redis** 
- 以特定格式显示：**docker images --format "{{.ID}}: {{.Repository}}"**

### ④ 给镜像打 Tag 

**docker tag <IMAGE ID> [<\**用户名\**>/]<镜像名>:<标签>**

镜像的唯一标识是其 ID 和摘要，而一个镜像可以有多个标签。

![](F:\MyGithub\Docker\Img\12.png)

### ⑤ 删除本地镜像 

**docker rmi [选项] <镜像1> [<镜像2> ...]**

<镜像> 可以是镜像短 ID、镜像长 ID、镜像名或者镜像摘要。

删除镜像的时候，实际上是在要求删除某个标签的镜像。所以首先需要做的是将满足我们要求的所有镜像标签都取消，这就是我们看到的Untagged 的信息。

镜像是多层存储结构，因此在删除的时候也是从上层向基础层方向依次进行判断删除。镜像的多层结构让镜像复用变得非常容易，因此很有可能某个其它镜像正依赖于当前镜像的某一层。这种情况，不会触发删除该层的行为。直到没有任何层依赖当前层时，才会真实的删除当前层。

除了镜像依赖以外，还需要注意的是容器对镜像的依赖。如果有用这个镜像启动的容器存在（即使容器没有运行），那么同样不可以删除这个镜像。如果这些容器是不需要的，应该先将它们删除，然后再来删除镜像。

![](F:\MyGithub\Docker\Img\13.png)

### ⑥ 批量删除镜像

删除所有虚悬镜像：**docker rmi $(docker images -q -f dangling=true)**

删除所有仓库名为 redis 的镜像：**docker rmi $(docker images -q redis)**

删除所有在 mysql:8.0 之前的镜像：**docker rmi $(docker images -q -f before=mysql:8.0)**

### ⑦ 导出镜像

**docker save -o <镜像文件> <镜像>**

可以将一个镜像完整的导出，就可以传输到其它地方去使用

![](F:\MyGithub\Docker\Img\14.png)

### ⑧ 导入镜像

**docker load -i <镜像文件>**

![](F:\MyGithub\Docker\Img\15.png)

### ⑨ 创建镜像

使用 Dockerfile 指令来创建一个新的镜像

 ***\*docker build\**** ， 从零开始来创建一个新的镜像。为此，我们需要创建一个 Dockerfile 文件，其中包含一组指令来告诉 Docker 如何构建我们的镜像。

```shell
#先通过vs创建Dockerfile文件，在创建镜像，core31v1.619镜像名称
docker build -t core31v1.619 -f Dockerfile .
```



# 容器管理

## 1、创建容器

启动容器有两种方式，一种是基于镜像新建一个容器并启动，另外一个是将在终止状态（stopped）的容器重新启动。Docker 容器非常轻量级，很多时候可以随时删除和新创建容器。

创建容器的主要命令为 **docker run (或 docker container run)**，常用参数如下：

![](F:\MyGithub\Docker\Img\17.png)

当利用 docker run 来创建容器时，Docker 在后台运行的标准操作包括：

- 检查本地是否存在指定的镜像，不存在就从公有仓库下载
- 利用镜像创建并启动一个容器
- 分配一个文件系统，并在只读的镜像层外面挂载一层可读写层
- 从宿主主机配置的网桥接口中桥接一个虚拟接口到容器中去
- 从地址池配置一个 ip 地址给容器
- 执行用户指定的应用程序
- 执行完毕后容器被终止

### ① 创建并进入容器

**docker run -ti <IMAGE ID OR NAME> /bin/bash**

创建容器，通过 **-ti** 参数分配一个 bash 终端，并进入容器内执行命令。退出容器时容器就被终止了。

![](F:\MyGithub\Docker\Img\18.png)

### ② 容器后台运

**docker run -d <IMAGE ID OR NAME>**

![](F:\MyGithub\Docker\Img\19.png)

容器是否会长久运行，是和 docker run 指定的命令有关，即容器内是否有前台进程在运行，和 -d 参数无关。一个容器必须有一个进程来守护容器才会在后台长久运行。

例如通过 -d 参数后台运行 centos，容器创建完成后将退出。而通过 -ti 分配一个伪终端后，容器内将有一个 bash 终端守护容器，容器不会退出。

![](F:\MyGithub\Docker\Img\20.png)

### ③ 进入容器

**docker exec -ti <CONTAINER ID> bash**

![](F:\MyGithub\Docker\Img\21.png)

### ④ 指定端口

**docker run -d -p <宿主机端口>:<容器内端口> <IMAGE ID>**

通过 -p 参数指定宿主机与容器内的端口映射，如果不指定宿主机端口，将会随机映射一个宿主机端口。映射好端口后，宿主机外部就可以通过该端口访问容器了

![](F:\MyGithub\Docker\Img\22.png)

### ⑤ 宿主机重启时自动重启容器

**docker run -d --restart always <IMAGE ID>** 

宿主机重启时，docker 容器默认不会自动启动，可以通过 --restart=always 设置自动重启。

## 2、容器资源限制

容器资源限制主要是对内存的限制和使用CPU数量的限制，常用选项如下：

![](F:\MyGithub\Docker\Img\23.png)

### ① 使用例子

1) 限制容器最多使用500M内存和100M的Swap，并禁用OOM Killer。注意 --memory-swap 的值为 --memory 的值加上 Swap 的值。

docker run -d --name nginx_1 --memory="500m" --memory-swap="600m" --oom-kill-disable nginx

如果 memory 小于等于 memory-swap，表示不使用Swap；如果 memory-swap="-1" 表示可以无限使用Swap；如果不设置 memory-swap，其值默认为 memory 的2倍。

2) 允许容器最多使用一个半的CPU：docker run -d --name nginx_2 --cpus="1.5" nginx

3) 允许容器最多使用 50% 的CPU：docker run -d --name nginx_3 --cpus=".5" nginx

### ② 查看容器资源使用统计

**docker stats <CONTAINER ID>**

通过 **docker stats** 可以实时查看容器资源使用情况。如果不对容器内存或CPU加以限制，将默认使用物理机所有内存和CPU。**通常情况下，为了安全一定要限制容器内存和CPU，否则容器内程序遭到攻击，可能无限占用物理机的内存和CPU**。

![](F:\MyGithub\Docker\Img\24.png)

## 3、容器常用命令

### ① 停止容器

**docker stop <CONTAINER ID>**

### ② 重启容器

**docker start \**<CONTAINER ID>\****

### ③ 删除容器

- 删除已停止的容器：

  ```
  docker rm <CONTAINER ID>
  ```

- 强制删除运行中的容器：

  ```
  docker rm -f <CONTAINER ID>
  ```

- 批量删除Exited(0)状态的容器：

  ```
  docker rm $(docker ps -aq -f exited=0)
  ```

- 批量删除所有容器：

  ```
  docker rm -f $(docker ps -aq)
  ```

### ④ 查看容器日志

**docker logs <CONTAINER ID OR NAMES>**

![](F:\MyGithub\Docker\Img\25.png)

### ⑤ 查看容器详细信息

**docker inspect <CONTAINER ID>**

### ⑥ 列出或指定容器端口映射

**docker port <CONTAINER ID>**

### ⑦ 显示一个容器运行的进程

**docker top <CONTAINER ID>**

### ⑧ 拷贝文件/文件夹到容器

**docker cp <dir> <CONTAINER ID>:<dir>**

![](F:\MyGithub\Docker\Img\26.png)

### ⑨ 查看容器与镜像的差异

**docker diff <CONTAINER ID>**

# 管理应用程序数据

容器删除后，里面的数据将一同被删除，因此需要将容器内经常变动的数据存储在容器之外，这样删除容器之后数据依然存在。

Docker 提供三种方式将数据从宿主机挂载到容器中

- **volumes**：由 Docker 管理的数据卷，是宿主机文件系统的一部分(/var/lib/docker/volumes)。是保存数据的最佳方式。
- **bind mounts**：将宿主机上的文件或目录挂载到容器中，通常在容器需要使用宿主机上的目录或文件时使用，比如搜集宿主机的信息、挂载宿主机上的 maven 仓库等。
- **tmpfs**：挂载存储在主机系统的内存中，而不会写入主机的文件系统。如果不希望将数据持久存储在任何位置，可以使用 tmpfs，同时避免写入容器可写层提高性能。这种方式使用比较少。

### 1、Volume

① 管理卷

创建数据卷：**docker volume create <volume_name>**

查看数据卷列表：**docker volume ls**

查看数据卷详细信息：**docker volume inspect <volume_name>**

创建的数据卷默认在宿主机的 /var/lib/docker/volumes/ 下

![](F:\MyGithub\Docker\Img\28.png)

② 创建容器时指定数据卷

1) 通过 --mount 方式：**--mount src=<数据卷名称>,dst=<容器内的数据目录>，注意逗号之间不能有空格**

docker run -d --name nginx_1 --mount src=nginx_html,dst=/usr/share/nginx/html nginx

2) 通过 -v 方式：**-v <数据卷名称>:<容器内的数据目录>**

docker run -d --name nginx_2 -v nginx_html:/usr/share/nginx/html nginx

以上两种方式，--mount 更加通用直观，-v 是老语法的方式。如果对应的数据卷不存在，将自动创建数据卷。如果挂载目标在容器中非空，则该目录现有内容会自动同步到数据卷中。

![](F:\MyGithub\Docker\Img\29.png)

③ 删除数据卷：**docker volume rm <volume_name>**

数据卷是独立于容器的生命周期的，删除容器是不会删除数据卷的，除非删除数据卷，删除数据卷之后数据也就丢失了。

如果数据卷正在被某个容器使用，将不能被删除，需要先删除使用此数据卷的所有容器之后才能删除数据卷。

![](F:\MyGithub\Docker\Img\30.png)

④ Volume 特点及使用场景

- 多个容器可以同时挂载相同的卷，可用于多个运行容器之间共享数据。
- 当容器停止或被删除后，数据卷依然存在。当明确删除卷时，卷才会被删除。
- 可以将容器的数据存储在远程主机或其它存储上。
- 将数据从一台 docker 主机迁移到另一台主机时，先停止容器，然后备份卷的目录 /var/lib/docker/volumes

### 2、Bind Mounts

① 创建容器时绑定数据卷

1) 通过 --mount 方式：**--mount type=bind,src=<宿主机目录>,dst=<容器内的数据目录>，注意逗号之间不能有空格**

docker run -d --name nginx_1 --mount type=bind,src=/data/nginx/html,dst=/usr/share/nginx/html nginx

2) 通过 -v 方式：**-v <宿主机目录>:\**<容器内的数据目录>\****

docker run -d --name nginx_2 -v /data/nginx/html:/usr/share/nginx/html nginx

以上两种方式，如果源文件/目录不存在，不会自动创建容器，会抛出错误。与 volume 方式相比，如果挂载目标在容器中非空，则该目录现有内容将被隐藏，可以理解成使用宿主机的目录覆盖了容器中的目录，新增的数据会同步。

![](F:\MyGithub\Docker\Img\31.png)

② Bind Mounts 特点及使用场景

- 从主机共享配置文件到容器。默认情况下，挂载主机 /etc/resolv.conf 到每个容器，提供 DNS 解析。
- 在 docker 主机上的开发环境和容器之间共享源代码。例如，可以将 maven target 目录挂载到容器中，每次在 docker 主机上构建maven项目时，容器都可以访问构建好的项目包。
- 当 docker 主机的文件或目
- 录结构保证与容器所需的绑定挂载一致时，例如容器中需要统计主机的一些信息，可以直接将主机的某些目录直接挂载给容器使用。

# .NetCore部署在Docker

## 第一种

① 发布.NetCore网站

②在云服务器新建目录/vip/wpublish，并拷贝到云服务器
![](F:\MyGithub\Docker\Img\32.png)

③ 创建容器实例

```shell
docker run -d -p 8081:80 -v /vip/wpublish:/app --workdir /app mcr.microsoft.com/dotnet/core/aspnet  dotnet /app/MyProject.WebMVC.dll
```

![](F:\MyGithub\Docker\Img\33.png)

④云服务器配置网络安全组-配置规则-入站端口

![](F:\MyGithub\Docker\Img\34.png)

⑤访问部署的网站

![](F:\MyGithub\Docker\Img\35.png)

## 第二种（Dockerfile）

①Visual Studio 创建dockerfile文件，网站点击添加，选择docker支持，选择Linx

![](F:\MyGithub\Docker\Img\36.png)

②阿里云服务器添加目录，并将项目拷贝到新建的目录，

![](F:\MyGithub\Docker\Img\38.png)

③ Dockerfile文件拷贝到sln解决方案目录下，Dockerfile文件是从该目录开始copy的

<img src="F:\MyGithub\Docker\Img\39.png" style="zoom:150%;" />

④创建镜像

```shell
docker build -t core31v1.619 -f Dockerfile .
#.表示当前目录
```

![](F:\MyGithub\Docker\Img\40.png)

 ⑤创建容器实例

```shell
docker run -itd -p 8082:80 core31v1.619
```

![](F:\MyGithub\Docker\Img\41.png)

# Nginx+集群

## ①搜索镜像

```
docker search nginx
```

![](F:\MyGithub\Docker\Img\42.png)

## ②下载镜像

```
docker pull nginx
```

## ③**查找容器中的nginx配置文件目录**

- 创建容器实例

```
docker run -d nginx
```

- 进入容器

```shell
docker exec -it 99bcc64c561a /bin/bash
#99bcc64c561a 容器id
```

![](F:\MyGithub\Docker\Img\45.png)

执行命令

```shell
# ls    查看该目录下的清淡
# cd etc  进入etc目录
# ls    
# cd nginx 进入nginx目录，即可看到nginx.conf配置文件
# cat nginx.conf  打开配置文件
# exit 退出容器
```

![](F:\MyGithub\Docker\Img\46.png)

## ④ 修改nginx配置文件.conf

#### 两种修改方式

方式一：直接在容器目录修改，首先要找到.conf文件的目录，找到.conf文件的目录后，由于不能够直接通过命令编辑，暂时采用第二种修改方式

方式二：从官网下载对应版本的配置文件.conf，修改配置后，替换容器中nginx的配置文件

①修改配置文件

![](F:\MyGithub\Docker\Img\43.png)

server:是部署的容器实例，listen：80 是容器内nginx监听的端口

②将修改后的配置文件拷贝到云服务器新建的目录/vip/wnginx

![](F:\MyGithub\Docker\Img\44.png)

## ⑤将修改后的配置文件挂载到容器实例

```shell
docker run -d -p 8086:80 -v /vip/wnginx/:/var/log/nginx/ -v /vip/wnginx/nginx.conf:/etc/nginx/nginx.conf:ro --name elnginx  nginx
#/vip/wnginx/:/var/log/nginx/ **将日志挂载到容器，nginx日志可以输出到ECS的nginx目录
```

![](F:\MyGithub\Docker\Img\47.png)

**nginx日志可以输出到ECS的nginx目录**

![](F:\MyGithub\Docker\Img\48.png)

# Redis

## ①搜索镜像

```
docker search redis
```

## ②下载镜像

```
docker pull redis
```

## ③**查找容器中的Redis配置文件目录**



## ④ 修改nginx配置文件.conf

```shell
bind 0.0.0.0 #默认是 bind 127.0.0.1，容器里面监听0.0.0.0
port 6380  #端口号
protected-mode no # yes本地连接，no远程连接
```

## ⑤将修改后的配置文件挂载到容器实例

```
docker run -d -p 8085:6380 -v /vip/redis/redis.conf:/usr/local/etc/redis/redis.conf -v /vip/redis/data:/data:rw --name redis02 redis:6.0 redis-server /usr/local/etc/redis/redis.conf
```

![](F:\MyGithub\Docker\Img\50.png)

## ⑥Redis Desktop Manager连接Redis

![](F:\MyGithub\Docker\Img\49.png)

# Docker管理工具

## Docker Compose（yml批量部署）

①下载

```
curl -L https://get.daocloud.io/docker/compose/releases/download/1.25.0/docker-compose-`uname -s`-`uname -m` > /usr/local/bin/docker-compose
```

②授权

```shell
chmod +x /usr/local/bin/docker-compose #授权
#docker-compose –version 查看版本
#docker-compose up –d 后台启动
#docker-compose stop  停止
```

③ yml配置

```shell
version: '3.3'
services:
  service1:
    build:
      context: /vip/wfile/Zhaoxi.AspNetCore31.DockerProject
    image: core31v1.619 
    ports:
      - 8081:80/tcp
  service2:
    image: core31v1.619
    ports:
      - 8082:80/tcp
    command: ["dotnet", "/app/Zhaoxi.AspNetCore31.DockerProject.dll"]
  nginx:
    image: nginx:latest
    ports:
      - 8086:80/tcp
    volumes:
      - /vip/wnginx/nginx.conf:/etc/nginx/nginx.conf
  redis:
    image: redis:6.0
    ports:
      - 8085:6380/tcp
    volumes:
      - /vip/redis/redis.conf:/usr/local/etc/redis/redis.conf
      - /vip/redis/data:/data:rw
    command: ["redis-server", "/usr/local/etc/redis/redis.conf"]
```

- image - 指定了我们用来运行容器的镜像，如果指定的image不存在，它会自动从远程仓库下载（上面的core31v1.619 是已经创建的镜像）
- ports - 指定了我们映射端口，这里把容器80端口映射到宿主机器8081/8082端口
- volumes - 指定了容器里的存储路径以volume挂载方式映射到宿主机器上

- context- 指定Dockerfile所在的位置

④将yml文件拷贝到/vip目录

![](F:\MyGithub\Docker\Img\51.png)

⑤执行命令

```shell
docker-compose up –d
```

