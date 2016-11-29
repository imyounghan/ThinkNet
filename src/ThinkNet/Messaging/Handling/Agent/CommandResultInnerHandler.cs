﻿using ThinkNet.Contracts;

namespace ThinkNet.Messaging.Handling.Agent
{
    /// <summary>
    /// 命令结果的内部处理器
    /// </summary>
    public class CommandResultInnerHandler : HandlerAgent//, IMessageHandler<CommandResult>
    {
        private readonly ICommandResultNotification _notification;

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        public CommandResultInnerHandler(ICommandResultNotification notification)
        {
            this._notification = notification;
        }

        public override object GetInnerHandler()
        {
            return this;
        }

        //public void Handle(params object[] args)
        //{            
        //    var reply = args[0] as CommandResult;

        //    this.TryHandle(reply);
        //}

        protected override void TryHandle(object[] args)
        {
            var result = args[0] as CommandResult;

            switch (result.CommandReturnType) {
                case CommandReturnType.CommandExecuted:
                    _notification.NotifyCommandHandled(result);
                    break;
                case CommandReturnType.DomainEventHandled:
                    _notification.NotifyEventHandled(result);
                    break;
            }
        }

        //#region IMessageHandler<CommandResult> 成员

        //void IMessageHandler<CommandResult>.Handle(CommandResult commandResult)
        //{
        //    this.TryHandle(commandResult);
        //}

        //#endregion
    }
}
