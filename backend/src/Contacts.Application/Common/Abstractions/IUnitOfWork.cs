using System;
using System.Collections.Generic;
using System.Text;

namespace Contacts.Application.Common.Abstractions
{
	public interface IUnitOfWork
	{
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
