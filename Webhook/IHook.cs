﻿namespace Webhook
{
    public interface IHook
    {
        void Notify(string key, object queryString = null, object body = null);
    }
}