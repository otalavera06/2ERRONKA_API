using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace ErronkaApi.Testak
{
    public class KategoriaKontrolTest
    {

        [Fact]
        public void get_kategoriak_badira_datu_basean()
        {
            var data = new List<Kategoria>()
            {
                new Kategoria(1, "Edariak"),
                new Kategoria(2, "Janaria")
            }.AsQueryable();

            var mockSession = new Mock<global::NHibernate.ISession>();

            mockSession
                .Setup(s => s.Query<Kategoria>())
                .Returns(data);

            var mockFactory = new Mock<global::NHibernate.ISessionFactory>();

            mockFactory
                .Setup(f => f.OpenSession())
                .Returns(mockSession.Object);

            var repo = new KategoriaRepository(mockFactory.Object);

            var controller = new KategoriaKontrollerra(repo);

            var result = controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(ok.Value);
        }



        [Fact]
        public void get_kategoriak_oker_datu_basean()
        {
            var data = new List<Kategoria>().AsQueryable();

            var mockSession = new Mock<global::NHibernate.ISession>();

            mockSession
                .Setup(s => s.Query<Kategoria>())
                .Returns(data);

            var mockFactory = new Mock<global::NHibernate.ISessionFactory>();

            mockFactory
                .Setup(f => f.OpenSession())
                .Returns(mockSession.Object);

            var repo = new KategoriaRepository(mockFactory.Object);

            var controller = new KategoriaKontrollerra(repo);

            var result = controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(ok.Value);
        }
    }
}