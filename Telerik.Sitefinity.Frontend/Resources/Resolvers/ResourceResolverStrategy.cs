﻿using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// This class implements a strategy for handling resource resolving.
    /// </summary>
    public class ResourceResolverStrategy : IResourceResolverStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceResolverStrategy"/> class.
        /// </summary>
        public ResourceResolverStrategy()
        {
            this.InitializeChain();
        }

        /// <inheritdoc />
        public IResourceResolverNode SetFirst(IResourceResolverNode resolver)
        {
            this.first = resolver;
            return this.First;
        }

        /// <inheritdoc />
        public IResourceResolverNode First
        {
            get
            {
                return this.first;
            }
        }

        /// <summary>
        /// Determines whether a file with the specified virtual path exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path to check.</param>
        public virtual bool Exists(PathDefinition definition, string virtualPath)
        {
            return this.First != null && this.First.Exists(definition, virtualPath);
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
        /// </returns>
        public virtual CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            this.AssertFirstNodeExists();

            return this.First.GetCacheDependency(definition, virtualPath, virtualPathDependencies, utcStart);
        }

        /// <summary>
        /// Opens the the file with the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file to open.</param>
        public virtual Stream Open(PathDefinition definition, string virtualPath)
        {
            this.AssertFirstNodeExists();

            return this.First.Open(definition, virtualPath);
        }

        /// <summary>
        /// Initializes the chain with the default nodes.
        /// </summary>
        protected virtual void InitializeChain()
        {
            var packagesManager = new PackagesManager();

            this.SetFirst(new FileSystemResourceResolver(() => packagesManager.GetCurrentPackageVirtualPath()))
                .SetNext(new DatabaseResourceResolver())
                .SetNext(new FileSystemResourceResolver())
                .SetNext(new EmbeddedResourceResolver());
        }

        private void AssertFirstNodeExists()
        {
            if (this.First == null)
            {
                throw new InvalidOperationException("Resource resolver strategy is empty.");
            }
        }

        private IResourceResolverNode first;
    }
}
