﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Server.HttpSys
{
    internal static unsafe class HttpApi
    {
        private const string HTTPAPI = "httpapi.dll";

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpInitialize(HTTPAPI_VERSION version, uint flags, void* pReserved);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpReceiveRequestEntityBody(SafeHandle requestQueueHandle, ulong requestId, uint flags, IntPtr pEntityBuffer, uint entityBufferLength, out uint bytesReturned, SafeNativeOverlapped pOverlapped);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpReceiveClientCertificate(SafeHandle requestQueueHandle, ulong connectionId, uint flags, HTTP_SSL_CLIENT_CERT_INFO* pSslClientCertInfo, uint sslClientCertInfoSize, uint* pBytesReceived, SafeNativeOverlapped pOverlapped);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpReceiveClientCertificate(SafeHandle requestQueueHandle, ulong connectionId, uint flags, byte* pSslClientCertInfo, uint sslClientCertInfoSize, uint* pBytesReceived, SafeNativeOverlapped pOverlapped);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpReceiveHttpRequest(SafeHandle requestQueueHandle, ulong requestId, uint flags, HTTP_REQUEST* pRequestBuffer, uint requestBufferLength, uint* pBytesReturned, SafeNativeOverlapped pOverlapped);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpSendHttpResponse(SafeHandle requestQueueHandle, ulong requestId, uint flags, HTTP_RESPONSE_V2* pHttpResponse, HTTP_CACHE_POLICY* pCachePolicy, uint* pBytesSent, SafeLocalFree pRequestBuffer, uint requestBufferLength, SafeNativeOverlapped pOverlapped, IntPtr pLogData);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpSendResponseEntityBody(SafeHandle requestQueueHandle, ulong requestId, uint flags, ushort entityChunkCount, HTTP_DATA_CHUNK* pEntityChunks, uint* pBytesSent, SafeLocalFree pRequestBuffer, uint requestBufferLength, SafeNativeOverlapped pOverlapped, IntPtr pLogData);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpCancelHttpRequest(SafeHandle requestQueueHandle, ulong requestId, IntPtr pOverlapped);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpWaitForDisconnectEx(SafeHandle requestQueueHandle, ulong connectionId, uint reserved, SafeNativeOverlapped overlapped);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpCreateServerSession(HTTPAPI_VERSION version, ulong* serverSessionId, uint reserved);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpCreateUrlGroup(ulong serverSessionId, ulong* urlGroupId, uint reserved);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint HttpAddUrlToUrlGroup(ulong urlGroupId, string pFullyQualifiedUrl, ulong context, uint pReserved);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpSetUrlGroupProperty(ulong urlGroupId, HTTP_SERVER_PROPERTY serverProperty, IntPtr pPropertyInfo, uint propertyInfoLength);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint HttpRemoveUrlFromUrlGroup(ulong urlGroupId, string pFullyQualifiedUrl, uint flags);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpCloseServerSession(ulong serverSessionId);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpCloseUrlGroup(ulong urlGroupId);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpSetRequestQueueProperty(SafeHandle requestQueueHandle, HTTP_SERVER_PROPERTY serverProperty, IntPtr pPropertyInfo, uint propertyInfoLength, uint reserved, IntPtr pReserved);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern unsafe uint HttpCreateRequestQueue(HTTPAPI_VERSION version, string pName,
            UnsafeNclNativeMethods.SECURITY_ATTRIBUTES pSecurityAttributes, uint flags, out HttpRequestQueueV2Handle pReqQueueHandle);

        [DllImport(HTTPAPI, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern unsafe uint HttpCloseRequestQueue(IntPtr pReqQueueHandle);

        internal enum HTTP_API_VERSION
        {
            Invalid,
            Version10,
            Version20,
        }

        // see http.w for definitions
        internal enum HTTP_SERVER_PROPERTY
        {
            HttpServerAuthenticationProperty,
            HttpServerLoggingProperty,
            HttpServerQosProperty,
            HttpServerTimeoutsProperty,
            HttpServerQueueLengthProperty,
            HttpServerStateProperty,
            HttpServer503VerbosityProperty,
            HttpServerBindingProperty,
            HttpServerExtendedAuthenticationProperty,
            HttpServerListenEndpointProperty,
            HttpServerChannelBindProperty,
            HttpServerProtectionLevelProperty,
        }

        // Currently only one request info type is supported but the enum is for future extensibility.

        internal enum HTTP_REQUEST_INFO_TYPE
        {
            HttpRequestInfoTypeAuth,
            HttpRequestInfoTypeChannelBind,
            HttpRequestInfoTypeSslProtocol,
            HttpRequestInfoTypeSslTokenBinding
        }

        internal enum HTTP_RESPONSE_INFO_TYPE
        {
            HttpResponseInfoTypeMultipleKnownHeaders,
            HttpResponseInfoTypeAuthenticationProperty,
            HttpResponseInfoTypeQosProperty,
        }

        internal enum HTTP_TIMEOUT_TYPE
        {
            EntityBody,
            DrainEntityBody,
            RequestQueue,
            IdleConnection,
            HeaderWait,
            MinSendRate,
        }

        internal const int MaxTimeout = 6;

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_VERSION
        {
            internal ushort MajorVersion;
            internal ushort MinorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_KNOWN_HEADER
        {
            internal ushort RawValueLength;
            internal sbyte* pRawValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct HTTP_DATA_CHUNK
        {
            [FieldOffset(0)]
            internal HTTP_DATA_CHUNK_TYPE DataChunkType;

            [FieldOffset(8)]
            internal FromMemory fromMemory;

            [FieldOffset(8)]
            internal FromFileHandle fromFile;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FromMemory
        {
            // 4 bytes for 32bit, 8 bytes for 64bit
            internal IntPtr pBuffer;
            internal uint BufferLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FromFileHandle
        {
            internal ulong offset;
            internal ulong count;
            internal IntPtr fileHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTPAPI_VERSION
        {
            internal ushort HttpApiMajorVersion;
            internal ushort HttpApiMinorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_COOKED_URL
        {
            internal ushort FullUrlLength;
            internal ushort HostLength;
            internal ushort AbsPathLength;
            internal ushort QueryStringLength;
            internal ushort* pFullUrl;
            internal ushort* pHost;
            internal ushort* pAbsPath;
            internal ushort* pQueryString;
        }

        // Only cache unauthorized GETs + HEADs.
        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_CACHE_POLICY
        {
            internal HTTP_CACHE_POLICY_TYPE Policy;
            internal uint SecondsToLive;
        }

        internal enum HTTP_CACHE_POLICY_TYPE : int
        {
            HttpCachePolicyNocache = 0,
            HttpCachePolicyUserInvalidates = 1,
            HttpCachePolicyTimeToLive = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SOCKADDR
        {
            internal ushort sa_family;
            internal byte sa_data;
            internal byte sa_data_02;
            internal byte sa_data_03;
            internal byte sa_data_04;
            internal byte sa_data_05;
            internal byte sa_data_06;
            internal byte sa_data_07;
            internal byte sa_data_08;
            internal byte sa_data_09;
            internal byte sa_data_10;
            internal byte sa_data_11;
            internal byte sa_data_12;
            internal byte sa_data_13;
            internal byte sa_data_14;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_TRANSPORT_ADDRESS
        {
            internal SOCKADDR* pRemoteAddress;
            internal SOCKADDR* pLocalAddress;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SSL_CLIENT_CERT_INFO
        {
            internal uint CertFlags;
            internal uint CertEncodedSize;
            internal byte* pCertEncoded;
            internal void* Token;
            internal byte CertDeniedByMapper;
        }

        internal enum HTTP_SERVICE_BINDING_TYPE : uint
        {
            HttpServiceBindingTypeNone = 0,
            HttpServiceBindingTypeW,
            HttpServiceBindingTypeA
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVICE_BINDING_BASE
        {
            internal HTTP_SERVICE_BINDING_TYPE Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST_CHANNEL_BIND_STATUS
        {
            internal IntPtr ServiceName;
            internal IntPtr ChannelToken;
            internal uint ChannelTokenSize;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_UNKNOWN_HEADER
        {
            internal ushort NameLength;
            internal ushort RawValueLength;
            internal sbyte* pName;
            internal sbyte* pRawValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SSL_INFO
        {
            internal ushort ServerCertKeySize;
            internal ushort ConnectionKeySize;
            internal uint ServerCertIssuerSize;
            internal uint ServerCertSubjectSize;
            internal sbyte* pServerCertIssuer;
            internal sbyte* pServerCertSubject;
            internal HTTP_SSL_CLIENT_CERT_INFO* pClientCertInfo;
            internal uint SslClientCertNegotiated;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_RESPONSE_HEADERS
        {
            internal ushort UnknownHeaderCount;
            internal HTTP_UNKNOWN_HEADER* pUnknownHeaders;
            internal ushort TrailerCount;
            internal HTTP_UNKNOWN_HEADER* pTrailers;
            internal HTTP_KNOWN_HEADER KnownHeaders;
            internal HTTP_KNOWN_HEADER KnownHeaders_02;
            internal HTTP_KNOWN_HEADER KnownHeaders_03;
            internal HTTP_KNOWN_HEADER KnownHeaders_04;
            internal HTTP_KNOWN_HEADER KnownHeaders_05;
            internal HTTP_KNOWN_HEADER KnownHeaders_06;
            internal HTTP_KNOWN_HEADER KnownHeaders_07;
            internal HTTP_KNOWN_HEADER KnownHeaders_08;
            internal HTTP_KNOWN_HEADER KnownHeaders_09;
            internal HTTP_KNOWN_HEADER KnownHeaders_10;
            internal HTTP_KNOWN_HEADER KnownHeaders_11;
            internal HTTP_KNOWN_HEADER KnownHeaders_12;
            internal HTTP_KNOWN_HEADER KnownHeaders_13;
            internal HTTP_KNOWN_HEADER KnownHeaders_14;
            internal HTTP_KNOWN_HEADER KnownHeaders_15;
            internal HTTP_KNOWN_HEADER KnownHeaders_16;
            internal HTTP_KNOWN_HEADER KnownHeaders_17;
            internal HTTP_KNOWN_HEADER KnownHeaders_18;
            internal HTTP_KNOWN_HEADER KnownHeaders_19;
            internal HTTP_KNOWN_HEADER KnownHeaders_20;
            internal HTTP_KNOWN_HEADER KnownHeaders_21;
            internal HTTP_KNOWN_HEADER KnownHeaders_22;
            internal HTTP_KNOWN_HEADER KnownHeaders_23;
            internal HTTP_KNOWN_HEADER KnownHeaders_24;
            internal HTTP_KNOWN_HEADER KnownHeaders_25;
            internal HTTP_KNOWN_HEADER KnownHeaders_26;
            internal HTTP_KNOWN_HEADER KnownHeaders_27;
            internal HTTP_KNOWN_HEADER KnownHeaders_28;
            internal HTTP_KNOWN_HEADER KnownHeaders_29;
            internal HTTP_KNOWN_HEADER KnownHeaders_30;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST_HEADERS
        {
            internal ushort UnknownHeaderCount;
            internal HTTP_UNKNOWN_HEADER* pUnknownHeaders;
            internal ushort TrailerCount;
            internal HTTP_UNKNOWN_HEADER* pTrailers;
            internal HTTP_KNOWN_HEADER KnownHeaders;
            internal HTTP_KNOWN_HEADER KnownHeaders_02;
            internal HTTP_KNOWN_HEADER KnownHeaders_03;
            internal HTTP_KNOWN_HEADER KnownHeaders_04;
            internal HTTP_KNOWN_HEADER KnownHeaders_05;
            internal HTTP_KNOWN_HEADER KnownHeaders_06;
            internal HTTP_KNOWN_HEADER KnownHeaders_07;
            internal HTTP_KNOWN_HEADER KnownHeaders_08;
            internal HTTP_KNOWN_HEADER KnownHeaders_09;
            internal HTTP_KNOWN_HEADER KnownHeaders_10;
            internal HTTP_KNOWN_HEADER KnownHeaders_11;
            internal HTTP_KNOWN_HEADER KnownHeaders_12;
            internal HTTP_KNOWN_HEADER KnownHeaders_13;
            internal HTTP_KNOWN_HEADER KnownHeaders_14;
            internal HTTP_KNOWN_HEADER KnownHeaders_15;
            internal HTTP_KNOWN_HEADER KnownHeaders_16;
            internal HTTP_KNOWN_HEADER KnownHeaders_17;
            internal HTTP_KNOWN_HEADER KnownHeaders_18;
            internal HTTP_KNOWN_HEADER KnownHeaders_19;
            internal HTTP_KNOWN_HEADER KnownHeaders_20;
            internal HTTP_KNOWN_HEADER KnownHeaders_21;
            internal HTTP_KNOWN_HEADER KnownHeaders_22;
            internal HTTP_KNOWN_HEADER KnownHeaders_23;
            internal HTTP_KNOWN_HEADER KnownHeaders_24;
            internal HTTP_KNOWN_HEADER KnownHeaders_25;
            internal HTTP_KNOWN_HEADER KnownHeaders_26;
            internal HTTP_KNOWN_HEADER KnownHeaders_27;
            internal HTTP_KNOWN_HEADER KnownHeaders_28;
            internal HTTP_KNOWN_HEADER KnownHeaders_29;
            internal HTTP_KNOWN_HEADER KnownHeaders_30;
            internal HTTP_KNOWN_HEADER KnownHeaders_31;
            internal HTTP_KNOWN_HEADER KnownHeaders_32;
            internal HTTP_KNOWN_HEADER KnownHeaders_33;
            internal HTTP_KNOWN_HEADER KnownHeaders_34;
            internal HTTP_KNOWN_HEADER KnownHeaders_35;
            internal HTTP_KNOWN_HEADER KnownHeaders_36;
            internal HTTP_KNOWN_HEADER KnownHeaders_37;
            internal HTTP_KNOWN_HEADER KnownHeaders_38;
            internal HTTP_KNOWN_HEADER KnownHeaders_39;
            internal HTTP_KNOWN_HEADER KnownHeaders_40;
            internal HTTP_KNOWN_HEADER KnownHeaders_41;
        }

        internal enum HTTP_VERB : int
        {
            HttpVerbUnparsed = 0,
            HttpVerbUnknown = 1,
            HttpVerbInvalid = 2,
            HttpVerbOPTIONS = 3,
            HttpVerbGET = 4,
            HttpVerbHEAD = 5,
            HttpVerbPOST = 6,
            HttpVerbPUT = 7,
            HttpVerbDELETE = 8,
            HttpVerbTRACE = 9,
            HttpVerbCONNECT = 10,
            HttpVerbTRACK = 11,
            HttpVerbMOVE = 12,
            HttpVerbCOPY = 13,
            HttpVerbPROPFIND = 14,
            HttpVerbPROPPATCH = 15,
            HttpVerbMKCOL = 16,
            HttpVerbLOCK = 17,
            HttpVerbUNLOCK = 18,
            HttpVerbSEARCH = 19,
            HttpVerbMaximum = 20,
        }

        internal static readonly string[] HttpVerbs = new string[]
        {
                null,
                "Unknown",
                "Invalid",
                "OPTIONS",
                "GET",
                "HEAD",
                "POST",
                "PUT",
                "DELETE",
                "TRACE",
                "CONNECT",
                "TRACK",
                "MOVE",
                "COPY",
                "PROPFIND",
                "PROPPATCH",
                "MKCOL",
                "LOCK",
                "UNLOCK",
                "SEARCH",
        };

        internal enum HTTP_DATA_CHUNK_TYPE : int
        {
            HttpDataChunkFromMemory = 0,
            HttpDataChunkFromFileHandle = 1,
            HttpDataChunkFromFragmentCache = 2,
            HttpDataChunkMaximum = 3,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_RESPONSE_INFO
        {
            internal HTTP_RESPONSE_INFO_TYPE Type;
            internal uint Length;
            internal HTTP_MULTIPLE_KNOWN_HEADERS* pInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_RESPONSE
        {
            internal uint Flags;
            internal HTTP_VERSION Version;
            internal ushort StatusCode;
            internal ushort ReasonLength;
            internal sbyte* pReason;
            internal HTTP_RESPONSE_HEADERS Headers;
            internal ushort EntityChunkCount;
            internal HTTP_DATA_CHUNK* pEntityChunks;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_RESPONSE_V2
        {
            internal HTTP_RESPONSE Response_V1;
            internal ushort ResponseInfoCount;
            internal HTTP_RESPONSE_INFO* pResponseInfo;
        }

        internal enum HTTP_RESPONSE_INFO_FLAGS : uint
        {
            None = 0,
            PreserveOrder = 1,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_MULTIPLE_KNOWN_HEADERS
        {
            internal HTTP_RESPONSE_HEADER_ID.Enum HeaderId;
            internal HTTP_RESPONSE_INFO_FLAGS Flags;
            internal ushort KnownHeaderCount;
            internal HTTP_KNOWN_HEADER* KnownHeaders;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST_AUTH_INFO
        {
            internal HTTP_AUTH_STATUS AuthStatus;
            internal uint SecStatus;
            internal uint Flags;
            internal HTTP_REQUEST_AUTH_TYPE AuthType;
            internal IntPtr AccessToken;
            internal uint ContextAttributes;
            internal uint PackedContextLength;
            internal uint PackedContextType;
            internal IntPtr PackedContext;
            internal uint MutualAuthDataLength;
            internal char* pMutualAuthData;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST_INFO
        {
            internal HTTP_REQUEST_INFO_TYPE InfoType;
            internal uint InfoLength;
            internal HTTP_REQUEST_AUTH_INFO* pInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST
        {
            internal uint Flags;
            internal ulong ConnectionId;
            internal ulong RequestId;
            internal ulong UrlContext;
            internal HTTP_VERSION Version;
            internal HTTP_VERB Verb;
            internal ushort UnknownVerbLength;
            internal ushort RawUrlLength;
            internal sbyte* pUnknownVerb;
            internal sbyte* pRawUrl;
            internal HTTP_COOKED_URL CookedUrl;
            internal HTTP_TRANSPORT_ADDRESS Address;
            internal HTTP_REQUEST_HEADERS Headers;
            internal ulong BytesReceived;
            internal ushort EntityChunkCount;
            internal HTTP_DATA_CHUNK* pEntityChunks;
            internal ulong RawConnectionId;
            internal HTTP_SSL_INFO* pSslInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST_V2
        {
            internal HTTP_REQUEST Request;
            internal ushort RequestInfoCount;
            internal HTTP_REQUEST_INFO* pRequestInfo;
        }

        internal enum HTTP_AUTH_STATUS
        {
            HttpAuthStatusSuccess,
            HttpAuthStatusNotAuthenticated,
            HttpAuthStatusFailure,
        }

        internal enum HTTP_REQUEST_AUTH_TYPE
        {
            HttpRequestAuthTypeNone = 0,
            HttpRequestAuthTypeBasic,
            HttpRequestAuthTypeDigest,
            HttpRequestAuthTypeNTLM,
            HttpRequestAuthTypeNegotiate,
            HttpRequestAuthTypeKerberos
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVER_AUTHENTICATION_INFO
        {
            internal HTTP_FLAGS Flags;
            internal HTTP_AUTH_TYPES AuthSchemes;
            internal bool ReceiveMutualAuth;
            internal bool ReceiveContextHandle;
            internal bool DisableNTLMCredentialCaching;
            internal ulong ExFlags;
            HTTP_SERVER_AUTHENTICATION_DIGEST_PARAMS DigestParams;
            HTTP_SERVER_AUTHENTICATION_BASIC_PARAMS BasicParams;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVER_AUTHENTICATION_DIGEST_PARAMS
        {
            internal ushort DomainNameLength;
            internal char* DomainName;
            internal ushort RealmLength;
            internal char* Realm;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVER_AUTHENTICATION_BASIC_PARAMS
        {
            ushort RealmLength;
            char* Realm;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_REQUEST_TOKEN_BINDING_INFO
        {
            public byte* TokenBinding;
            public uint TokenBindingSize;

            public byte* TlsUnique;
            public uint TlsUniqueSize;

            public char* KeyType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_TIMEOUT_LIMIT_INFO
        {
            internal HTTP_FLAGS Flags;
            internal ushort EntityBody;
            internal ushort DrainEntityBody;
            internal ushort RequestQueue;
            internal ushort IdleConnection;
            internal ushort HeaderWait;
            internal uint MinSendRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_BINDING_INFO
        {
            internal HTTP_FLAGS Flags;
            internal IntPtr RequestQueueHandle;
        }

        // see http.w for definitions
        [Flags]
        internal enum HTTP_FLAGS : uint
        {
            NONE = 0x00000000,
            HTTP_RECEIVE_REQUEST_FLAG_COPY_BODY = 0x00000001,
            HTTP_RECEIVE_SECURE_CHANNEL_TOKEN = 0x00000001,
            HTTP_SEND_RESPONSE_FLAG_DISCONNECT = 0x00000001,
            HTTP_SEND_RESPONSE_FLAG_MORE_DATA = 0x00000002,
            HTTP_SEND_RESPONSE_FLAG_BUFFER_DATA = 0x00000004,
            HTTP_SEND_RESPONSE_FLAG_RAW_HEADER = 0x00000004,
            HTTP_SEND_REQUEST_FLAG_MORE_DATA = 0x00000001,
            HTTP_PROPERTY_FLAG_PRESENT = 0x00000001,
            HTTP_INITIALIZE_SERVER = 0x00000001,
            HTTP_INITIALIZE_CBT = 0x00000004,
            HTTP_SEND_RESPONSE_FLAG_OPAQUE = 0x00000040,
        }

        [Flags]
        internal enum HTTP_AUTH_TYPES : uint
        {
            NONE = 0x00000000,
            HTTP_AUTH_ENABLE_BASIC = 0x00000001,
            HTTP_AUTH_ENABLE_DIGEST = 0x00000002,
            HTTP_AUTH_ENABLE_NTLM = 0x00000004,
            HTTP_AUTH_ENABLE_NEGOTIATE = 0x00000008,
            HTTP_AUTH_ENABLE_KERBEROS = 0x00000010,
        }

        private const int HttpHeaderRequestMaximum = (int)HttpSysRequestHeader.UserAgent + 1;
        private const int HttpHeaderResponseMaximum = (int)HttpSysResponseHeader.WwwAuthenticate + 1;

        internal static class HTTP_REQUEST_HEADER_ID
        {
            internal static string ToString(int position)
            {
                return _strings[position];
            }

            private static string[] _strings =
            {
                    "Cache-Control",
                    "Connection",
                    "Date",
                    "Keep-Alive",
                    "Pragma",
                    "Trailer",
                    "Transfer-Encoding",
                    "Upgrade",
                    "Via",
                    "Warning",

                    "Allow",
                    "Content-Length",
                    "Content-Type",
                    "Content-Encoding",
                    "Content-Language",
                    "Content-Location",
                    "Content-MD5",
                    "Content-Range",
                    "Expires",
                    "Last-Modified",

                    "Accept",
                    "Accept-Charset",
                    "Accept-Encoding",
                    "Accept-Language",
                    "Authorization",
                    "Cookie",
                    "Expect",
                    "From",
                    "Host",
                    "If-Match",

                    "If-Modified-Since",
                    "If-None-Match",
                    "If-Range",
                    "If-Unmodified-Since",
                    "Max-Forwards",
                    "Proxy-Authorization",
                    "Referer",
                    "Range",
                    "Te",
                    "Translate",
                    "User-Agent",
                };
        }

        internal static class HTTP_RESPONSE_HEADER_ID
        {
            private static string[] _strings =
            {
                    "Cache-Control",
                    "Connection",
                    "Date",
                    "Keep-Alive",
                    "Pragma",
                    "Trailer",
                    "Transfer-Encoding",
                    "Upgrade",
                    "Via",
                    "Warning",

                    "Allow",
                    "Content-Length",
                    "Content-Type",
                    "Content-Encoding",
                    "Content-Language",
                    "Content-Location",
                    "Content-MD5",
                    "Content-Range",
                    "Expires",
                    "Last-Modified",

                    "Accept-Ranges",
                    "Age",
                    "ETag",
                    "Location",
                    "Proxy-Authenticate",
                    "Retry-After",
                    "Server",
                    "Set-Cookie",
                    "Vary",
                    "WWW-Authenticate",
                };

            private static Dictionary<string, int> _lookupTable = CreateLookupTable();

            private static Dictionary<string, int> CreateLookupTable()
            {
                Dictionary<string, int> lookupTable = new Dictionary<string, int>((int)Enum.HttpHeaderResponseMaximum, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < (int)Enum.HttpHeaderResponseMaximum; i++)
                {
                    lookupTable.Add(_strings[i], i);
                }
                return lookupTable;
            }

            internal static int IndexOfKnownHeader(string HeaderName)
            {
                int index;
                return _lookupTable.TryGetValue(HeaderName, out index) ? index : -1;
            }

            internal static string ToString(int position)
            {
                return _strings[position];
            }

            internal enum Enum
            {
                HttpHeaderCacheControl = 0,    // general-header [section 4.5]
                HttpHeaderConnection = 1,    // general-header [section 4.5]
                HttpHeaderDate = 2,    // general-header [section 4.5]
                HttpHeaderKeepAlive = 3,    // general-header [not in rfc]
                HttpHeaderPragma = 4,    // general-header [section 4.5]
                HttpHeaderTrailer = 5,    // general-header [section 4.5]
                HttpHeaderTransferEncoding = 6,    // general-header [section 4.5]
                HttpHeaderUpgrade = 7,    // general-header [section 4.5]
                HttpHeaderVia = 8,    // general-header [section 4.5]
                HttpHeaderWarning = 9,    // general-header [section 4.5]

                HttpHeaderAllow = 10,   // entity-header  [section 7.1]
                HttpHeaderContentLength = 11,   // entity-header  [section 7.1]
                HttpHeaderContentType = 12,   // entity-header  [section 7.1]
                HttpHeaderContentEncoding = 13,   // entity-header  [section 7.1]
                HttpHeaderContentLanguage = 14,   // entity-header  [section 7.1]
                HttpHeaderContentLocation = 15,   // entity-header  [section 7.1]
                HttpHeaderContentMd5 = 16,   // entity-header  [section 7.1]
                HttpHeaderContentRange = 17,   // entity-header  [section 7.1]
                HttpHeaderExpires = 18,   // entity-header  [section 7.1]
                HttpHeaderLastModified = 19,   // entity-header  [section 7.1]

                // Response Headers

                HttpHeaderAcceptRanges = 20,   // response-header [section 6.2]
                HttpHeaderAge = 21,   // response-header [section 6.2]
                HttpHeaderEtag = 22,   // response-header [section 6.2]
                HttpHeaderLocation = 23,   // response-header [section 6.2]
                HttpHeaderProxyAuthenticate = 24,   // response-header [section 6.2]
                HttpHeaderRetryAfter = 25,   // response-header [section 6.2]
                HttpHeaderServer = 26,   // response-header [section 6.2]
                HttpHeaderSetCookie = 27,   // response-header [not in rfc]
                HttpHeaderVary = 28,   // response-header [section 6.2]
                HttpHeaderWwwAuthenticate = 29,   // response-header [section 6.2]

                HttpHeaderResponseMaximum = 30,

                HttpHeaderMaximum = 41
            }
        }

        private static HTTPAPI_VERSION version;

        // This property is used by HttpListener to pass the version structure to the native layer in API
        // calls. 

        internal static HTTPAPI_VERSION Version
        {
            get
            {
                return version;
            }
        }

        // This property is used by HttpListener to get the Api version in use so that it uses appropriate 
        // Http APIs.

        internal static HTTP_API_VERSION ApiVersion
        {
            get
            {
                if (version.HttpApiMajorVersion == 2 && version.HttpApiMinorVersion == 0)
                {
                    return HTTP_API_VERSION.Version20;
                }
                else if (version.HttpApiMajorVersion == 1 && version.HttpApiMinorVersion == 0)
                {
                    return HTTP_API_VERSION.Version10;
                }
                else
                {
                    return HTTP_API_VERSION.Invalid;
                }
            }
        }

        static HttpApi()
        {
            InitHttpApi(2, 0);
        }

        private static void InitHttpApi(ushort majorVersion, ushort minorVersion)
        {
            version.HttpApiMajorVersion = majorVersion;
            version.HttpApiMinorVersion = minorVersion;

            var statusCode = HttpInitialize(version, (uint)HTTP_FLAGS.HTTP_INITIALIZE_SERVER, null);

            supported = statusCode == UnsafeNclNativeMethods.ErrorCodes.ERROR_SUCCESS;
        }

        private static volatile bool supported;
        internal static bool Supported
        {
            get
            {
                return supported;
            }
        }
    }
}
