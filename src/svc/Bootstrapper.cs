using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BryanPorter.SlackMeme.Service.Models;
using Nancy;
using Nancy.Bootstrappers.Ninject;
using Ninject;

namespace BryanPorter.SlackMeme.Service
{
    public class Bootstrapper
        : NinjectNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(IKernel existingContainer)
        {
            existingContainer.Bind<ICommandParser>().To<CommandParser>().InSingletonScope();
        }

        protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
        {
            container.Bind<IBlobStore>().To<ImageStore>();
            container.Bind<IImageGenerator>().To<ImageGenerator>();
            container.Bind<IImageProvider>().To<ImageProvider>();
        }
    }
}
