using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Support.CreateSupportTicket;

public sealed record CreateSupportTicketUseCase(CreateSupportTicketDto TicketData)
	: IRequest<ApiBaseResponse>;