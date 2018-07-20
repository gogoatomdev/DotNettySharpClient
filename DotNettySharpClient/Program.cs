using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace DotNettySharpClient
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("start connecting");
            try
            {
                RunClientAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        static async Task RunClientAsync()
        {
            ExampleHelper.SetConsoleLogger();

            var group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            if (ClientSettings.IsSsl)
            {
                cert = new X509Certificate2(Path.Combine(ExampleHelper.ProcessDirectory, "dotnetty.com.pfx"), "password");
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        if (cert != null)
                        {
                            pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                        }

                        //pipeline.AddLast(new MyClientHanlder());
                        pipeline.AddLast("frameDecoder", new LengthFieldBasedFrameDecoder(1024, 0, 4, 0, 4));
                        pipeline.AddLast("frameEncoder", new LengthFieldPrepender(4));
                        pipeline.AddLast("decoder", new StringDecoder(Encoding.UTF8));
                        pipeline.AddLast("encoder", new StringEncoder(Encoding.UTF8));

                        pipeline.AddLast(new EchoClientHandler());
                    }));


                IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(ClientSettings.Host, ClientSettings.Port));

                Console.ReadLine();

                await bootstrapChannel.CloseAsync();
            }
            finally
            {
                group.ShutdownGracefullyAsync().Wait(10000);
            }
        }

        public static void SendMsg(String msg)
        {

        }

        //public static void Main() => RunClientAsync().Wait();
    }
}
