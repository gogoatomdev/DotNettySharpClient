using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace DotNettySharpClient
{
    class EchoClientHandler : ChannelHandlerAdapter
    {
        readonly IByteBuffer initialMessage;
        int postTimes = 0;
        public EchoClientHandler()

        {
            this.initialMessage = Unpooled.Buffer(ClientSettings.Size);
            byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world123");
            this.initialMessage.WriteBytes(messageBytes);
        }
        //重写基类方法，当链接上服务器后，马上发送Hello World消息到服务端
        public override void ChannelActive(IChannelHandlerContext context) => context.WriteAndFlushAsync(this.initialMessage);

        public override void Flush(IChannelHandlerContext context)
        {
            base.Flush(context);
        }



        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            // var byteBuffer = message as IByteBuffer;
            // if (byteBuffer != null)
            // {
            //     Console.WriteLine("-----Received from server: " + byteBuffer.ToString(Encoding.UTF8));
            // }
            //context.WriteAsync(message);

            if (message != null)
            {
                Console.WriteLine("-----Received from server: " + message.ToString());
            }

            if (this.postTimes < 100)
            {
                context.WriteAsync("多次数据传输测试： " + this.postTimes.ToString());
                this.postTimes++;
            }

        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush(); public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}
